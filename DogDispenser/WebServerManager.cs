using nanoFramework.WebServer;
using System;
using System.Diagnostics;
using System.Net;

namespace DogDispenser
{
    public class WebServerManager
    {
        private WebServer _server;
        private readonly StepperMotorManager _stepperMotorManager;
        private readonly ScheduleManager _scheduleManager;
        private readonly LedManager _ledManager;

        public WebServerManager(StepperMotorManager stepperMotorManager, ScheduleManager scheduleManager, LedManager ledManager)
        {
            _stepperMotorManager = stepperMotorManager;
            _scheduleManager = scheduleManager;
            _ledManager = ledManager;
        }

        public void Start()
        {
            try
            {
                _server = new WebServer(80, HttpProtocol.Http);
                _server.CommandReceived += OnCommandReceived;
                _server.Start();

                Debug.WriteLine($"Web Server started on port 80");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start web server: {ex.Message}");
                throw;
            }
        }

        public void Stop()
        {
            if (_server == null) return;

            _server.Stop();
            _server.CommandReceived -= OnCommandReceived;
            _server = null;

            Debug.WriteLine("Web Server stopped");
        }

        private void OnCommandReceived(object sender, WebServerEventArgs e)
        {
            try
            {
                Debug.WriteLine($"Request: {e.Context.Request.HttpMethod} {e.Context.Request.RawUrl}");

                var url = e.Context.Request.RawUrl;

                switch (url)
                {
                    case "/dispense":
                        Dispense();
                        break;
                    case "/forward":
                        Forward();
                        break;
                    case "/backward":
                        Backward();
                        break;
                }

                if (url.StartsWith("/setSchedule1"))
                    SetScheduleFromUrl(url, 1);

                if (url.StartsWith("/setSchedule2"))
                    SetScheduleFromUrl(url, 2);

                if (url.StartsWith("/getSchedules"))
                {
                    var schedule1 = _scheduleManager.GetSchedule1();
                    var schedule2 = _scheduleManager.GetSchedule2();
                    var response = $"{{\"schedule1\":\"{schedule1}\",\"schedule2\":\"{schedule2}\"}}";
                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
                }

                WebServer.OutputAsStream(e.Context.Response, HtmlContent.HomePage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private void Dispense()
        {
            _ledManager.OnOperating();

            _stepperMotorManager.Forward();
            _stepperMotorManager.Backward();

            _ledManager.OnOperationComplete();
        }

        private void Forward()
        {
            _ledManager.OnOperating();

            _stepperMotorManager.Forward();

            _ledManager.OnOperationComplete();
        }

        private void Backward()
        {
            _ledManager.OnOperating();

            _stepperMotorManager.Backward();

            _ledManager.OnOperationComplete();
        }

        private void SetScheduleFromUrl(string url, int scheduleId)
        {
            try
            {
                var hour = GetIntFromUrl(url, "hour");
                var minute = GetIntFromUrl(url, "minute");
                var enabled = GetIntFromUrl(url, "enabled");

                if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60) return;

                if(scheduleId == 1) 
                    _scheduleManager.SetSchedule1(hour, minute, enabled == 1);
                else 
                    _scheduleManager.SetSchedule2(hour, minute, enabled == 1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting schedule 1: {ex.Message}");
            }
        }

        private int GetIntFromUrl(string url, string paramName)
        {
            try
            {
                var index = url.IndexOf(paramName + "=");
                var valueStr = url.Substring(index + paramName.Length + 1);

                if (index >= 0)
                {
                    var endIndex = valueStr.IndexOf('&');
                    if (endIndex >= 0) valueStr = valueStr.Substring(0, endIndex);
                    return int.Parse(valueStr);
                }
            }
            catch { }
            return 0;
        }
    }
}