#![allow(unused_imports)]
use std::fs;
use std::io::{Read, Write};
use std::net::{TcpListener, TcpStream};
use std::path::PathBuf;
use std::sync::{Condvar, Mutex};
use std::{cell::RefCell, env, sync::atomic::*, sync::Arc, thread, time::*};

use anyhow::bail;

use embedded_svc::mqtt::client::utils::ConnState;
use log::*;

use url;

use smol;

use embedded_hal::adc::OneShot;
use embedded_hal::blocking::delay::DelayMs;
use embedded_hal::digital::v2::OutputPin;

use embedded_svc::eth;
use embedded_svc::eth::{Eth, TransitionalState};
use embedded_svc::httpd::registry::*;
use embedded_svc::httpd::*;
use embedded_svc::io;
use embedded_svc::io::Read as OtherRead;
use embedded_svc::ipv4;
use embedded_svc::mqtt::client::{Client, Connection, MessageImpl, Publish, QoS};
use embedded_svc::ping::Ping;
use embedded_svc::sys_time::SystemTime;
use embedded_svc::timer::TimerService;
use embedded_svc::timer::*;
use embedded_svc::wifi::*;

use esp_idf_svc::eth::*;
use esp_idf_svc::eventloop::*;
use esp_idf_svc::eventloop::*;
use esp_idf_svc::httpd as idf;
use esp_idf_svc::httpd::ServerRegistry;
use esp_idf_svc::mqtt::client::*;
use esp_idf_svc::netif::*;
use esp_idf_svc::nvs::*;
use esp_idf_svc::ping;
use esp_idf_svc::sntp;
use esp_idf_svc::sysloop::*;
use esp_idf_svc::systime::EspSystemTime;
use esp_idf_svc::timer::*;
use esp_idf_svc::wifi::*;

use esp_idf_hal::adc;
use esp_idf_hal::delay;
use esp_idf_hal::gpio;
use esp_idf_hal::i2c;
use esp_idf_hal::prelude::*;
use esp_idf_hal::spi;

use esp_idf_sys::{self, c_types};
use esp_idf_sys::{esp, EspError};

use display_interface_spi::SPIInterfaceNoCS;

use embedded_graphics::mono_font::{ascii::FONT_10X20, MonoTextStyle};
use embedded_graphics::pixelcolor::*;
use embedded_graphics::prelude::*;
use embedded_graphics::primitives::*;
use embedded_graphics::text::*;

#[cfg(esp32s2)]
include!(env!("EMBUILD_GENERATED_SYMBOLS_FILE"));

#[cfg(esp32s2)]
const ULP: &[u8] = include_bytes!(env!("EMBUILD_GENERATED_BIN_FILE"));

const SSID: &str = env!("RUST_ESP32_STD_DEMO_WIFI_SSID");
const PASS: &str = env!("RUST_ESP32_STD_DEMO_WIFI_PASS");

fn main() -> Result<()> {
    // Bind the log crate to the ESP Logging facilities
    esp_idf_svc::log::EspLogger::initialize_default();
    info!("Starting main()");

    #[allow(unused)]
    let netif_stack = Arc::new(EspNetifStack::new()?);
    #[allow(unused)]
    let sys_loop_stack = Arc::new(EspSysLoopStack::new()?);
    #[allow(unused)]
    let default_nvs = Arc::new(EspDefaultNvs::new()?);

    #[allow(clippy::redundant_clone)]
    #[allow(unused_mut)]
    let mut wifi_result = create_wifi(
        netif_stack.clone(),
        sys_loop_stack.clone(),
        default_nvs.clone(),
    );

    match wifi_result {
        Ok(wifi) => {
            test_https_client()?;
            {
                info!("Dropping wifi.");
                drop(wifi);
                info!("Wifi dropped.");
            }
        },
        Err(e) => {
            error!("Wifi initialization failed: {e}");
        }
    }
    
    info!("About to get to sleep now. Will wake up automatically in 1 minutes.");
    unsafe { esp_idf_sys::esp_deep_sleep(Duration::from_secs(60).as_micros() as u64); }

    Ok(())
}

#[allow(dead_code)]
fn create_wifi(
    netif_stack: Arc<EspNetifStack>,
    sys_loop_stack: Arc<EspSysLoopStack>,
    default_nvs: Arc<EspDefaultNvs>,
) -> Result<Box<EspWifi>> {
    let mut wifi = Box::new(EspWifi::new(netif_stack, sys_loop_stack, default_nvs)?);

    info!("Wifi created, about to scan");

    let ap_infos = wifi.scan()?;

    let ours = ap_infos.into_iter().find(|a| a.ssid == SSID);

    let channel = if let Some(ours) = ours {
        info!(
            "Found configured access point '{}' on channel {}.",
            SSID, ours.channel
        );
        Some(ours.channel)
    } else {
        info!(
            "Configured access point '{}' not found during scanning, will go with unknown channel.",
            SSID
        );
        None
    };

    wifi.set_configuration(&Configuration::Client(
        ClientConfiguration {
            ssid: SSID.into(),
            password: PASS.into(),
            channel,
            ..Default::default()
        }))?;

    info!("Wifi configuration set, about to get status");

    thread::sleep(Duration::from_secs(1));

    let now = Instant::now();
    let result = wifi.wait_status_with_timeout(Duration::from_secs(180), |status| !status.is_transitional());
    let elapsed = now.elapsed();
    info!("Elapsed: {:.2?}", elapsed);

    result.map_err(|e| anyhow::anyhow!("Unexpected Wifi status 1: {:?}", e))?;

    let status = wifi.get_status();

    if let Status(
        ClientStatus::Started(ClientConnectionStatus::Connected(ClientIpStatus::Done(_ip_settings))),
        ApStatus::Stopped,
    ) = status
    {
        info!("Wifi connected");
    } else {
        bail!("Unexpected Wifi status 2: {:?}", status);
    }    

    Ok(wifi)
}

fn test_https_client() -> anyhow::Result<()> {
    use embedded_svc::http::{self, client::*, status, Headers, Status};
    use embedded_svc::io;
    use esp_idf_svc::http::client::*;

    let url = String::from("https://periodiccheckin.azurewebsites.net/api/HttpTrigger3?code=IvtGrjf2PcLRABnPGOxDnwZJp-h5HghhrriNn64xr2z0AzFuzxoBlw==&name=DavesTinyPico");

    info!("About to fetch content from {}", url);

    let mut client = EspHttpClient::new(&EspHttpClientConfiguration {
        crt_bundle_attach: Some(esp_idf_sys::esp_crt_bundle_attach),

        ..Default::default()
    })?;

    // let peripherals = esp_idf_hal::peripherals::Peripherals::take().unwrap();
    // let mut led = peripherals.pins.gpio33.into_output()?;
    // led.set_high()?;

    let mut response = client.get(&url)?.submit()?;

    let mut body = [0_u8; 3048];

    io::read_max(response.reader(), &mut body)?;

    let utf8_result = String::from_utf8((&body).to_vec());

    // led.set_low()?;

    match utf8_result {
        Ok(v) => {
            let v = v.trim_matches(char::from(0));
            info!(
                "Body (truncated to 3K):\n{:?}", v
            );
        },
        Err(e) => info!("Invalid UTF-8 sequence: {}", e)

    }

    Ok(())
}
