using System;
using System.Device.Gpio;
using System.Diagnostics;

namespace DogDispenser
{
    public class Stepper : IDisposable
    {
        private readonly GpioController _gpio;
        private readonly int _motorPin1;
        private readonly int _motorPin2;
        private readonly int _motorPin3;
        private readonly int _motorPin4;
        private readonly int _numberOfSteps;

        private int _stepNumber;
        private int _direction;
        private long _lastStepTime;
        private long _stepDelay;

        public Stepper(int numberOfSteps, int motorPin1, int motorPin2, int motorPin3, int motorPin4)
        {
            _stepNumber = 0;
            _direction = 0;
            _lastStepTime = 0;
            _numberOfSteps = numberOfSteps;

            _motorPin1 = motorPin1;
            _motorPin2 = motorPin2;
            _motorPin3 = motorPin3;
            _motorPin4 = motorPin4;

            _gpio = new GpioController();
            _gpio.OpenPin(_motorPin1, PinMode.Output);
            _gpio.OpenPin(_motorPin2, PinMode.Output);
            _gpio.OpenPin(_motorPin3, PinMode.Output);
            _gpio.OpenPin(_motorPin4, PinMode.Output);
        }

        public void SetSpeed(long whatSpeed)
        {
            // real microsecond delay
            _stepDelay = 60_000_000L / _numberOfSteps / whatSpeed;
        }

        public void Step(int stepsToMove)
        {
            int stepsLeft = Math.Abs(stepsToMove);

            _direction = stepsToMove > 0 ? 1 : 0;

            while (stepsLeft > 0)
            {
                long now = Micros();

                if (now - _lastStepTime >= _stepDelay)
                {
                    _lastStepTime = now;

                    if (_direction == 1)
                    {
                        _stepNumber++;
                        if (_stepNumber == _numberOfSteps)
                            _stepNumber = 0;
                    }
                    else
                    {
                        if (_stepNumber == 0)
                            _stepNumber = _numberOfSteps;
                        _stepNumber--;
                    }

                    stepsLeft--;

                    StepMotor(_stepNumber % 4);
                }
            }
        }

        private long Micros()
        {
            long ticks = Stopwatch.GetTimestamp();
            return ticks * 1_000_000L / Stopwatch.Frequency;
        }

        private void StepMotor(int thisStep)
        {
            switch (thisStep)
            {
                case 0:  // 1010
                    _gpio.Write(_motorPin1, PinValue.High);
                    _gpio.Write(_motorPin2, PinValue.Low);
                    _gpio.Write(_motorPin3, PinValue.High);
                    _gpio.Write(_motorPin4, PinValue.Low);
                    break;

                case 1:  // 0110
                    _gpio.Write(_motorPin1, PinValue.Low);
                    _gpio.Write(_motorPin2, PinValue.High);
                    _gpio.Write(_motorPin3, PinValue.High);
                    _gpio.Write(_motorPin4, PinValue.Low);
                    break;

                case 2:  // 0101
                    _gpio.Write(_motorPin1, PinValue.Low);
                    _gpio.Write(_motorPin2, PinValue.High);
                    _gpio.Write(_motorPin3, PinValue.Low);
                    _gpio.Write(_motorPin4, PinValue.High);
                    break;

                case 3:  // 1001
                    _gpio.Write(_motorPin1, PinValue.High);
                    _gpio.Write(_motorPin2, PinValue.Low);
                    _gpio.Write(_motorPin3, PinValue.Low);
                    _gpio.Write(_motorPin4, PinValue.High);
                    break;
            }
        }

        public void Dispose()
        {
            _gpio?.Dispose();
        }
    }
}
