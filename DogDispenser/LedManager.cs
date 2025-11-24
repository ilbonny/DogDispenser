using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace DogDispenser
{
    public class LedManager
    {
        private const int RED_LED_PIN = 14;
        private const int GREEN_LED_PIN = 15;

        private GpioController _gpio;
        private Thread _blinkThread;
        private bool _isBlinking;
        private bool _isGreenBlinking;
        private Thread _greenBlinkThread;

        public LedManager()
        {
            _gpio = new GpioController();
            _gpio.OpenPin(RED_LED_PIN, PinMode.Output);
            _gpio.OpenPin(GREEN_LED_PIN, PinMode.Output);

            SetRedLed(false);
            SetGreenLed(false);

            Debug.WriteLine("LED Manager inizializzato");
        }

        public void OnSystemStarting()
        {
            SetRedLed(true);
        }

        public void OnWifiConnecting()
        {
            StartRedBlinking(200); 
        }

        public void OnWifiConnected()
        {
            StopRedBlinking();
            StopGreenBlinking();
            Thread.Sleep(1000);
            SetRedLed(false);
            SetGreenLed(true);
        }

        public void OnWifiDisconnected()
        {
            StopRedBlinking();
            StopGreenBlinking();
            Thread.Sleep(1000);
            SetGreenLed(false);
            SetRedLed(true);
        }

        public void WaitingForNetworkCardInformation()
        {
            StopRedBlinking();
            StartGreenBlinking(200);
        }

        public void OnOperating()
        {
            StartGreenBlinking(200); 
        }

        public void OnOperationComplete()
        {
            StopGreenBlinking();
            Thread.Sleep(1000);
            SetGreenLed(true);
        }
        
        public void SetRedLed(bool state)
        {
            _gpio.Write(RED_LED_PIN, state ? PinValue.High : PinValue.Low); 
        }

        public void SetGreenLed(bool state)
        {
            _gpio.Write(GREEN_LED_PIN, state ? PinValue.High : PinValue.Low);
        }

        private void StartRedBlinking(int intervalMs)
        {
            StopRedBlinking();

            _isBlinking = true;
            _blinkThread = new Thread(() =>
            {
                while (_isBlinking)
                {
                    SetRedLed(true);
                    Thread.Sleep(intervalMs);

                    SetRedLed(false);
                    Thread.Sleep(intervalMs);
                }
            });

            _blinkThread.Start();
        }

        private void StopRedBlinking()
        {
            if (_blinkThread == null) return;

            _isBlinking = false;
            Thread.Sleep(100);
            _blinkThread = null;
        }

        private void StartGreenBlinking(int intervalMs)
        {
            StopGreenBlinking();

            _isGreenBlinking = true;
            _greenBlinkThread = new Thread(() =>
            {
                while (_isGreenBlinking)
                {
                    SetGreenLed(true);
                    Thread.Sleep(intervalMs);

                    SetGreenLed(false);
                    Thread.Sleep(intervalMs);
                }
            });

            _greenBlinkThread.Start();
        }

        private void StopGreenBlinking()
        {
            if (_greenBlinkThread == null) return;

            _isGreenBlinking = false;
            Thread.Sleep(100);
            _greenBlinkThread = null;
        }

        public void Dispose()
        {
            StopRedBlinking();
            StopGreenBlinking();
            _gpio?.Dispose();
        }
    }
}