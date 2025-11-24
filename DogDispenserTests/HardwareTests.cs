using System.Diagnostics;
using System.Threading;
using DogDispenser;
using nanoFramework.TestFramework;

namespace DogDispenserTests
{
    [TestClass]
    public class HardwareTests
    {
        [Setup]
        public void Setup()
        {
            OutputHelper.WriteLine("=================================");
            OutputHelper.WriteLine("Assicurati che il dispositivo sia connesso!");
            OutputHelper.WriteLine("=================================");
        }

        [TestMethod]
        public void Motor_CompleteRotation_ShouldDispenseFood()
        {
            // Test manuale con hardware reale
            var motor = new StepperMotorManager();

            OutputHelper.WriteLine("Test iniziato - Verifica erogazione cibo");
            Debug.WriteLine("Test iniziato - Verifica erogazione cibo");
            motor.Forward();
            Thread.Sleep(5000);

            OutputHelper.WriteLine("✅ Test completato - Verifica manualmente l'erogazione");
            // Verifica manuale che il cibo sia stato erogato
        }

        [TestMethod]
        public void LED_VisualTest_ShouldShowCorrectStates()
        {
            var led = new LedManager();

            OutputHelper.WriteLine("🔴 LED Rosso ON - Verifica visivamente");
            Debug.WriteLine("Rosso ON - Verifica visivamente");
            Thread.Sleep(2000);

            led.OnWifiConnected();
            OutputHelper.WriteLine("🟢 LED Verde ON, Rosso OFF - Verifica visivamente");
            Debug.WriteLine("Verde ON, Rosso OFF - Verifica visivamente");
            Thread.Sleep(2000);

            led.OnWifiDisconnected();
            OutputHelper.WriteLine("🔴 LED Rosso ON, Verde OFF - Verifica visivamente");
            Thread.Sleep(2000);

            OutputHelper.WriteLine("✅ Test completato");
        }

        [Cleanup]
        public void Cleanup()
        {
            OutputHelper.WriteLine("Cleanup completato");
        }
    }
}