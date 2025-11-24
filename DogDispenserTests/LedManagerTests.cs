using DogDispenser;
using nanoFramework.TestFramework;

namespace DogDispenserTests
{
    [TestClass]
    public class LedManagerTests
    {
        [TestMethod]
        public void LedManager_Constructor_RedLedShouldBeOn()
        {
            var ledManager = new LedManager();

            // Verifica che il LED rosso sia acceso all'avvio
        }

        [TestMethod]
        public void OnWifiConnected_ShouldTurnOnGreenLed()
        {
            var ledManager = new LedManager();

            ledManager.OnWifiConnected();

            // Verifica che il LED verde sia acceso e il rosso spento
        }
    }
}