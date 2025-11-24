using DogDispenser;
using nanoFramework.TestFramework;

namespace DogDispenserTests
{
    [TestClass]
    public class ScheduleMotorIntegrationTests
    {
        [TestMethod]
        public void ScheduleManager_ExecuteSchedule_ShouldActivateMotor()
        {
            // Arrange
            var motorManager = new StepperMotorManager();
            //var scheduleManager = new ScheduleManager(motorManager, );

            // Act
            // Aggiungi uno schedule e verifica l'esecuzione

            // Assert
            // Verifica che il motore sia stato attivato
        }
    }
}