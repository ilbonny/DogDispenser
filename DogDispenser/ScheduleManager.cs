using System;
using System.Diagnostics;
using System.Threading;

namespace DogDispenser
{
    public class ScheduleManager
    {
        private readonly StepperMotorManager _stepperMotorManager;
        private readonly LedManager _ledManager;
        private TimeSpan _schedule1;
        private TimeSpan _schedule2;
        private bool _schedule1Enabled;
        private bool _schedule2Enabled;
        private Thread _schedulerThread;
        private bool _isRunning;

        public ScheduleManager(StepperMotorManager stepperMotorManager, LedManager ledManager)
        {
            _stepperMotorManager = stepperMotorManager;
            _ledManager = ledManager;
            _schedule1 = new TimeSpan(12, 0, 0); 
            _schedule2 = new TimeSpan(20, 0, 0); 
            _schedule1Enabled = false;
            _schedule2Enabled = false;
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _schedulerThread = new Thread(SchedulerLoop);
            _schedulerThread.Start();
            Debug.WriteLine("Schedule Manager started");
        }

        public void Stop()
        {
            _isRunning = false;
            if (_schedulerThread != null)
            {
                _schedulerThread = null;
            }
            Debug.WriteLine("Schedule Manager stopped");
        }

        public void SetSchedule1(int hour, int minute, bool enabled)
        {
            _schedule1 = new TimeSpan(hour, minute, 0);
            _schedule1Enabled = enabled;
            Debug.WriteLine($"Schedule 1 set to {hour:D2}:{minute:D2}, Enabled: {enabled}");
        }

        public void SetSchedule2(int hour, int minute, bool enabled)
        {
            _schedule2 = new TimeSpan(hour, minute, 0);
            _schedule2Enabled = enabled;
            Debug.WriteLine($"Schedule 2 set to {hour:D2}:{minute:D2}, Enabled: {enabled}");
        }

        public string GetSchedule1()
        {
            return $"{_schedule1.Hours:D2}:{_schedule1.Minutes:D2}|{(_schedule1Enabled ? "1" : "0")}";
        }

        public string GetSchedule2()
        {
            return $"{_schedule2.Hours:D2}:{_schedule2.Minutes:D2}|{(_schedule2Enabled ? "1" : "0")}";
        }

        private void SchedulerLoop()
        {
            var lastCheck = DateTime.UtcNow;

            while (_isRunning)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var currentTime = now.TimeOfDay;

                    if (now.Minute != lastCheck.Minute)
                    {
                        if (_schedule1Enabled &&
                            currentTime.Hours == _schedule1.Hours &&
                            currentTime.Minutes == _schedule1.Minutes)
                        {
                            Debug.WriteLine("⏰ Schedule 1 triggered - Dispensing food!");
                            DispenseFood();
                        }

                        if (_schedule2Enabled &&
                            currentTime.Hours == _schedule2.Hours &&
                            currentTime.Minutes == _schedule2.Minutes)
                        {
                            Debug.WriteLine("⏰ Schedule 2 triggered - Dispensing food!");
                            DispenseFood();
                        }

                        lastCheck = now;
                    }

                    Thread.Sleep(1000); 
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Scheduler error: {ex.Message}");
                }
            }
        }

        private void DispenseFood()
        {
            try
            {
                _ledManager.OnOperating();
                _stepperMotorManager.Forward();

                Thread.Sleep(100);

                _stepperMotorManager.Backward();

                _ledManager.OnOperationComplete();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Dispense error: {ex.Message}");
            }
        }
    }
}