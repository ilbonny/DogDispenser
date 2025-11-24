using DogDispenser;
using nanoFramework.TestFramework;

namespace DogDispenserTests
{
    [TestClass]
    public class StepperMotorManagerTests
    {
        [TestMethod]
        public void Forward_ShouldMove2048Steps()
        {
            // Arrange
            var motorManager = new StepperMotorManager();

            // Act
            motorManager.Forward();

            // Assert
            // Verifica che il motore abbia fatto 2048 passi
        }

        [TestMethod]
        public void Backward_ShouldMoveNegative2048Steps()
        {
            // Arrange
            var motorManager = new StepperMotorManager();

            // Act
            motorManager.Backward();

            // Assert
            // Verifica movimento all'indietro
        }
    }
}