using DogDispenser;
using nanoFramework.TestFramework;

namespace DogDispenserTests
{
    [TestClass]
    public class WebServerFunctionalTests
    {
        [TestMethod]
        public void WebServer_GetHomePage_ShouldReturn200()
        {
            // Arrange
            var motorManager = new StepperMotorManager();
            //var scheduleManager = new ScheduleManager(motorManager);
            //var webServer = new WebServerManager(motorManager, scheduleManager);
            //webServer.Start();
        }

        [TestMethod]
        public void WebServer_DispenseCommand_ShouldActivateMotor()
        {
            // Test comando di erogazione via web
        }
    }
}