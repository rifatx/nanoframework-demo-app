using nanoFramework.Networking;
using nanoFramework.Runtime.Native;
using System.Device.Gpio;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;

namespace NFTest1.Wifi
{
    internal class WifiManager
    {
        private const int SETUP_PIN = 23;

        private static readonly WebServer _server;

        static WifiManager()
        {
            _server = new();
        }

        public void Connect()
        {
            //var t = new Thread(() =>
            //{
            //    ConnectInternal();
            //});

            //t.Start();
            //t.Join();
            ConnectInternal();
        }

        private void ConnectInternal()
        {
            var gpioController = new GpioController();
            var setupButton = gpioController.OpenPin(SETUP_PIN, PinMode.Input);

            if (!Wireless80211.IsEnabled() || setupButton.Read() == PinValue.High)
            {
                Debug.WriteLine("Creating AP");
                LedController.Instance.DoublePulse();

                Wireless80211.Disable();

                if (!WirelessAP.Setup())
                {
                    Debug.WriteLine($"Setup Soft AP, Rebooting device");
                    Power.RebootDevice();
                }

                Debug.WriteLine($"Running Soft AP, waiting for client to connect");
                Debug.WriteLine($"Soft AP IP address :{WirelessAP.GetIP()}");

                _server.Start();
            }
            else
            {
                Debug.WriteLine("Connecting to wifi");
                Debug.WriteLine($"DHCP: {Wireless80211.GetInterface().IsDhcpEnabled}");
                LedController.Instance.SinglePulse();

                CancellationTokenSource cs = new(60000);
                var conf = Wireless80211.GetConfiguration();
                Wireless80211.GetInterface().EnableDhcp();
                var success = WifiNetworkHelper.ScanAndConnectDhcp(conf.Ssid, conf.Password, requiresDateTime: true, token: cs.Token);

                if (!success)
                {
                    Debug.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");

                    if (WifiNetworkHelper.HelperException != null)
                    {
                        Debug.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
                    }

                    return;
                }

                Debug.WriteLine($"Connected! IP adress: {Wireless80211.GetInterface().IPv4Address}");
            }
        }
    }
}
