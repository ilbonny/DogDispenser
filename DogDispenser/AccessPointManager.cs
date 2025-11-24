using Iot.Device.DhcpServer;
using nanoFramework.Runtime.Native;
using System;
using System.Device.Wifi;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace DogDispenser
{
    public class AccessPointManager
    {
        private WifiAdapter _wifiAdapter;

        public string IpAddress { get; private set; }
        public bool IsActive { get; private set; }
        public string Ssid { get; private set; }

        private const string DEFAULT_AP_SSID = "DogDispenser";
        private const string DEFAULT_AP_IP = "192.168.4.1";

        public void Start()
        {
            if (Setup() == false)
            {
                Console.WriteLine($"Rebooting device");
                Power.RebootDevice();
            }

            Thread.Sleep(3000);

            VerifyAccessPointStatus();

            var dhcpserver = new DhcpServer { CaptivePortalUrl = $"http://{DEFAULT_AP_IP}" };

            var dhcpInitResult = dhcpserver.Start(IPAddress.Parse(DEFAULT_AP_IP),
                new IPAddress(new byte[] { 255, 255, 255, 0 }));
            if (!dhcpInitResult)
            {
                Console.WriteLine($"Error initializing DHCP server.");
                Power.RebootDevice();
            }

            Console.WriteLine($"Running DogDispenser AP, waiting for client to connect");
            Debug.WriteLine("Informazioni Access Point:");
            Debug.WriteLine($"SSID: {DEFAULT_AP_SSID}");
            
            Debug.WriteLine("=================================");
            Debug.WriteLine("✅ ACCESS POINT OPERATIVO");
            Debug.WriteLine($"SSID: {DEFAULT_AP_SSID}");
            Debug.WriteLine($"IP: {DEFAULT_AP_IP}");
            Debug.WriteLine("=================================");
            Console.WriteLine($"Running DogDispenser AP, waiting for client to connect");

        }

        public bool Setup()
        {
            var ni = GetInterface();
            var wapconf = GetConfiguration();

            if (wapconf.Options == (WirelessAPConfiguration.ConfigurationOptions.Enable |
                                    WirelessAPConfiguration.ConfigurationOptions.AutoStart) &&
                ni.IPv4Address == DEFAULT_AP_IP)
            {
                return true;
            }

            ni.EnableStaticIPv4(DEFAULT_AP_IP, "255.255.255.0", DEFAULT_AP_IP);

            wapconf.Options = WirelessAPConfiguration.ConfigurationOptions.AutoStart |
                              WirelessAPConfiguration.ConfigurationOptions.Enable;
            wapconf.Ssid = DEFAULT_AP_SSID;
            wapconf.MaxConnections = 1;
            wapconf.Authentication = System.Net.NetworkInformation.AuthenticationType.WPA2;
            wapconf.Password ="dogdispenser123";
            wapconf.SaveConfiguration();

            return false;
        }

        public WirelessAPConfiguration GetConfiguration()
        {
            var ni = GetInterface();
            return WirelessAPConfiguration.GetAllWirelessAPConfigurations()[ni.SpecificConfigId];
        }

        public NetworkInterface GetInterface()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var ni in interfaces)
            {
                if (ni.NetworkInterfaceType != NetworkInterfaceType.WirelessAP) continue;
                return ni;
            }

            return null;
        }

        private void VerifyAccessPointStatus()
        {
            Debug.WriteLine("🔍 Verifica stato Access Point...");

            try
            {
                var ni = GetInterface();
                if (ni == null)
                {
                    Debug.WriteLine("⚠️ WARNING: Interfaccia non disponibile dopo avvio");
                    return;
                }

                var config = GetConfiguration();
                if (config == null)
                {
                    Debug.WriteLine("⚠️ WARNING: Configurazione non disponibile");
                    return;
                }

                Debug.WriteLine("--- STATO ACCESS POINT ---");
                Debug.WriteLine($"Interfaccia:");
                Debug.WriteLine($"   IP: {ni.IPv4Address}");
                Debug.WriteLine($"   Subnet: {ni.IPv4SubnetMask}");
                Debug.WriteLine($"   Gateway: {ni.IPv4GatewayAddress}");

                Debug.WriteLine($"Configurazione AP:");
                Debug.WriteLine($"   SSID: {config.Ssid}");
                Debug.WriteLine($"   Enabled: {(config.Options & WirelessAPConfiguration.ConfigurationOptions.Enable) != 0}");
                Debug.WriteLine($"   AutoStart: {(config.Options & WirelessAPConfiguration.ConfigurationOptions.AutoStart) != 0}");
                Debug.WriteLine($"   Authentication: {config.Authentication}");
                Debug.WriteLine("-------------------------");

                if (ni.IPv4Address != DEFAULT_AP_IP) 
                    Debug.WriteLine($"⚠️ WARNING: IP non corretto! Atteso {DEFAULT_AP_IP}, trovato {ni.IPv4Address}");

                
                if ((config.Options & WirelessAPConfiguration.ConfigurationOptions.Enable) == 0) 
                    Debug.WriteLine("⚠️ WARNING: Access Point NON abilitato!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Errore durante verifica: {ex.Message}");
            }
        }
    }
}