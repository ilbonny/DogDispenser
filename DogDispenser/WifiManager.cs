using System;
using System.Device.Wifi;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;

namespace DogDispenser
{
    public class WiFiManager
    {
        private readonly LedManager _ledManager;
        private WifiAdapter _wifiAdapter;

        public WiFiManager(LedManager ledManager)
        {
            _ledManager = ledManager;
        }

        public string IpAddress { get; private set; }
        public bool IsConnected { get; private set; }

        public bool Connect(string ssid, string password)
        {
            try
            {
                Debug.WriteLine($"Connessione a WiFi: {ssid}...");
                Debug.WriteLine("Waiting for WiFi adapter to be ready...");
                Thread.Sleep(10000);

                _wifiAdapter = WifiAdapter.FindAllAdapters()[0];

               // ScanNetworks();

                var result = _wifiAdapter.Connect(ssid, WifiReconnectionKind.Automatic, password);

                _ledManager.WaitingForNetworkCardInformation();

                Thread.Sleep(5000);

                if (!GetNetworkInfo())
                {
                    IsConnected = false;
                    return false;
                }

                IsConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRORE durante la connessione WiFi: {ex.Message}");
                IsConnected = false;
                return false;
            }
        }

        private bool GetNetworkInfo()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var ni in interfaces)
                {
                    if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211) continue;

                    IpAddress = ni.IPv4Address;

                    if (IpAddress == "0.0.0.0") return false;

                    Debug.WriteLine("=================================");
                    Debug.WriteLine("WiFi Connesso!");
                    Debug.WriteLine($"Indirizzo IP: {ni.IPv4Address}");
                    Debug.WriteLine($"Subnet Mask: {ni.IPv4SubnetMask}");
                    Debug.WriteLine($"Gateway: {ni.IPv4GatewayAddress}");
                    Debug.WriteLine("=================================");
                    Debug.WriteLine($"Apri il browser a: http://{ni.IPv4Address}");
                    Debug.WriteLine("=================================");

                    return true;
                }

                Debug.WriteLine("ERRORE: Interfaccia WiFi non trovata!");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRORE nell'ottenere le informazioni di rete: {ex.Message}");
                return false;
            }
        }
  
        public void Disconnect()
        {
            try
            {
                if (_wifiAdapter == null) return;

                _wifiAdapter.Disconnect();
                IsConnected = false;
                IpAddress = null;

                Debug.WriteLine("WiFi disconnesso");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRORE durante la disconnessione WiFi: {ex.Message}");
            }
        }

        public void ScanNetworks()
        {
            try
            {
                Debug.WriteLine("Scansione reti WiFi in corso...");

                _wifiAdapter.ScanAsync();

                Thread.Sleep(5000);

                var networks = _wifiAdapter.NetworkReport.AvailableNetworks;
                Debug.WriteLine($"Trovate {networks.Length} reti:");

                foreach (var network in networks)
                {
                    Debug.WriteLine($"- SSID: {network.Ssid}, Segnale: {network.NetworkRssiInDecibelMilliwatts} dBm");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRORE durante la scansione: {ex.Message}");
            }
        }
    }
}