
using Iot.Device.Uln2003;
using System.Diagnostics;

namespace DogDispenser
{
    public class StepperMotorManager
    {
        // ULN2003 Motor Driver Pins
        const int IN1 = 19;
        const int IN2 = 18;

        const int IN3 = 5;
        const int IN4 = 17;

        private Uln2003 _motor;

        public StepperMotorManager()
        {
            // Initialize motor
            _motor = new Uln2003(IN1, IN2, IN3, IN4);
            _motor.Mode = StepperMode.FullStepDualPhase;
            _motor.RPM = 15;

            Debug.WriteLine("Motor initialized");
        }

        public void Forward()
        {
            _motor.Step(2048);
        }

        public void Backward()
        {
            _motor.Step(-2048);
        }
    }
}
