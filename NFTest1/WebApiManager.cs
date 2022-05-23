using nanoFramework.WebServer;
using NFTest1.ApiControllers;
using System;

namespace NFTest1
{
    internal class WebApiManager
    {
        private WebServer _server;

        public void StartApi()
        {
            _server = new WebServer(8001, HttpProtocol.Http, new Type[] { typeof(HealthCheckController), typeof(RelayController) });
            _server.Start();
        }

        public void StopApi()
        {
            _server.Stop();
            _server.Dispose();
        }
    }
}
