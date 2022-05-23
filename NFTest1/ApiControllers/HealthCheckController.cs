using nanoFramework.WebServer;

namespace NFTest1.ApiControllers
{
    internal class HealthCheckController
    {
        [Route("health")]
        [Method("GET")]
        public void RouteGetHealth(WebServerEventArgs wsea)
        {
            LedController.Instance.Flash(new ILedCommand[]
            {
                new OnCommand(),
                new DelayCommand(200),
                new OffCommand(),
                new DelayCommand(100),
                new OnCommand(),
                new DelayCommand(200),
                new OffCommand(),
                new DelayCommand(100),
                new OnCommand(),
                new DelayCommand(200),
                new OffCommand()
            });
            wsea.Context.Response.ContentType = "text/plain";
            WebServer.OutPutStream(wsea.Context.Response, "OK");
        }
    }
}

