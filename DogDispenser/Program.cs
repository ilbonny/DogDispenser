using System.Diagnostics;
using System.Threading;

namespace DogDispenser
{
    public class Program
    {
        const string WIFI_SSID = "MOVISTAR_C92D";
        const string WIFI_PASSWORD = "";

        const bool USE_ACCESS_POINT_MODE = false;

        public static void Main()
        {
            Debug.WriteLine("=================================");
            Debug.WriteLine("   Dog Dispenser Web Server");
            Debug.WriteLine("=================================");

            Init();
        }

        private static void Init()
        {
            var ledManager = new LedManager();
            ledManager.OnSystemStarting();

            var stepperMotorManager = new StepperMotorManager();
            var scheduleManager = new ScheduleManager(stepperMotorManager, ledManager);
            var webServerManager = new WebServerManager(stepperMotorManager, scheduleManager, ledManager);

            ledManager.OnWifiConnecting();

            if (USE_ACCESS_POINT_MODE)
            {
                InitWithAccessPoint(ledManager, scheduleManager, webServerManager);
            }
            else
            {
                InitWithWiFiClient(ledManager, scheduleManager, webServerManager);
            }
        }

        private static void InitWithWiFiClient(LedManager ledManager, ScheduleManager scheduleManager, WebServerManager webServerManager)
        {
            Debug.WriteLine("Modalità: WiFi Client");

            var wifiManager = new WiFiManager(ledManager);

            ledManager.OnWifiConnecting();

            if (!wifiManager.Connect(WIFI_SSID, WIFI_PASSWORD))
            {
                Debug.WriteLine("ERROR: Failed to connect to WiFi!");
                ledManager.OnWifiDisconnected();
                wifiManager.Disconnect();
                return;
            }

            ledManager.OnWifiConnected();
            scheduleManager.Start();
            webServerManager.Start();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void InitWithAccessPoint(LedManager ledManager, ScheduleManager scheduleManager,
            WebServerManager webServerManager)
        {
            Debug.WriteLine("Modalità: Access Point");
                
            var apManager = new AccessPointManager();

            ledManager.OnWifiConnecting();

            apManager.Start();

            ledManager.OnWifiConnected();

            scheduleManager.Start();
            webServerManager.Start();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}