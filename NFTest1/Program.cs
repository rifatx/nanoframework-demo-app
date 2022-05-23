using NFTest1.Wifi;
using System.Diagnostics;
using System.Threading;

namespace NFTest1
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine($"Booting! Device name");

            var wm = new WifiManager();
            wm.Connect();


            var wam = new WebApiManager();
            wam.StartApi();

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
