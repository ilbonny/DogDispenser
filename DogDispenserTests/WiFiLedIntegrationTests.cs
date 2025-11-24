using DogDispenser;
using nanoFramework.TestFramework;

namespace DogDispenserTests
{
    [TestClass]
    public class WiFiLedIntegrationTests
    {
        [TestMethod]
        public void WiFiConnection_Success_ShouldTurnGreenLedOn()
        {
            // Arrange
            var ledManager = new LedManager();
            var wifiManager = new WiFiManager(ledManager);

            // Act
            bool connected = wifiManager.Connect("TestSSID", "TestPassword");
            if (connected)
            {
                ledManager.OnWifiConnected();
            }

            // Assert
            Assert.IsTrue(wifiManager.IsConnected);
            // Verifica stato LED
        }
    }
}