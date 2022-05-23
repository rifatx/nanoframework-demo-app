using nanoFramework.WebServer;
using System;
using System.Collections;
using System.Device.Gpio;
using System.Diagnostics;
using System.Text;

namespace NFTest1.ApiControllers
{
    internal class RelayController
    {
        private const string RELAY_ROUTE = "relay";
        private const string PAR_PIN_NO = "pinNo";
        private const string PAR_COMMAND = "cmd";
        private const string CMD_ON = "on";
        private const string CMD_OFF = "off";

        private static readonly GpioController _gpioController;

        static RelayController()
        {
            _gpioController = new GpioController();
        }

        [Route(RELAY_ROUTE)]
        [Method("POST")]
        public void RouteSetRelay(WebServerEventArgs wsea)
        {
            var pd = new Hashtable();

            foreach (var par in WebServer.DecodeParam(wsea.Context.Request.RawUrl))
            {
                pd.Add(par.Name, par.Value);
            }

            var sb = new StringBuilder();

            if (pd[PAR_PIN_NO] is var pp && pp is null)
            {
                sb.AppendLine($"Parameter {PAR_PIN_NO} is mandatory");
            }

            if (!int.TryParse(pp?.ToString(), out var pinNo))
            {
                sb.AppendLine("Invalid pinNo");
            }

            if (pd[PAR_COMMAND] is var cmd && cmd is null)
            {
                sb.AppendLine($"Parameter {PAR_COMMAND} is mandatory");
            }

            wsea.Context.Response.ContentType = "text/plain";

            if (sb.Length > 0)
            {
                var errors = sb.ToString();
                Debug.WriteLine($"Errors:");
                Debug.WriteLine(errors);
                wsea.Context.Response.StatusCode = 400;
                WebServer.OutPutStream(wsea.Context.Response, errors);

                return;
            }

            try
            {
                switch (cmd)
                {
                    case CMD_ON:
                        SetPinValue(pinNo, PinValue.Low);
                        break;
                    case CMD_OFF:
                        SetPinValue(pinNo, PinValue.High);
                        break;
                    default:
                        var err = $"Unknown pin command: {cmd}";
                        Debug.WriteLine(err);
                        wsea.Context.Response.StatusCode = 400;
                        WebServer.OutPutStream(wsea.Context.Response, err);
                        return;
                }

                WebServer.OutPutStream(wsea.Context.Response, $"pin: {pinNo}, cmd: {cmd}");
                return;
            }
            catch (Exception ex)
            {
                var err = ex.ToString();

                Debug.WriteLine(err);

                WebServer.OutPutStream(wsea.Context.Response, $"Error: {err}");
                return;
            }

        }

        private void SetPinValue(int pinNo, PinValue pinValue)
        {
            if (!_gpioController.IsPinOpen(pinNo))
            {
                _gpioController.OpenPin(pinNo, PinMode.Output);
            }

            _gpioController.Write(pinNo, pinValue);
        }
    }
}
