﻿using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using nanoFramework.Runtime.Native;

namespace NFTest1.Wifi
{
    public class WebServer
    {
        HttpListener _listener;
        Thread _serverThread;

        public void Start()
        {
            if (_listener == null)
            {
                _listener = new HttpListener("http");
                _serverThread = new Thread(RunServer);
                _serverThread.Start();
            }
        }

        public void Stop()
        {
            if (_listener != null)
                _listener.Stop();
        }
        private void RunServer()
        {
            _listener.Start();

            while (_listener.IsListening)
            {
                var context = _listener.GetContext();
                if (context != null)
                    ProcessRequest(context);
            }
            _listener.Close();

            _listener = null;
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            string responseString;
            string ssid = null;
            string password = null;
            bool isApSet = false;

            switch (request.HttpMethod)
            {
                case "GET":
                    string[] url = request.RawUrl.Split('?');
                    if (url[0] == "/favicon.ico")
                    {
                        response.StatusCode = 404;
                        //response.ContentType = "image/png";
                        //byte[] responseBytes = Resources.GetBytes(Resources.BinaryResources.favicon);
                        //OutPutByteResponse(response, responseBytes);
                    }
                    else
                    {
                        response.ContentType = "text/html";
                        responseString = ReplaceMessage(Constants.MAIN_HTML, "");
                        OutPutResponse(response, responseString);
                    }
                    break;

                case "POST":
                    Hashtable hashPars = ParseParamsFromStream(request.InputStream);
                    ssid = (string)hashPars["ssid"];
                    password = (string)hashPars["password"];

                    Debug.WriteLine($"Wireless parameters SSID:{ssid} PASSWORD:{password}");

                    string message = "<p>New settings saved.</p><p>Rebooting device to put into normal mode</p>";

                    responseString = CreateMainPage(message);

                    OutPutResponse(response, responseString);
                    isApSet = true;
                    break;
            }

            response.Close();

            if (isApSet && (!string.IsNullOrEmpty(ssid)) && (!string.IsNullOrEmpty(password)))
            {
                Wireless80211.Configure(HttpUtility.UrlDecode(ssid), HttpUtility.UrlDecode(password));

                WirelessAP.Disable();
                Thread.Sleep(200);
                Power.RebootDevice();
            }
        }

        static string ReplaceMessage(string page, string message)
        {
            int index = page.IndexOf("{message}");
            if (index >= 0)
            {
                return page.Substring(0, index) + message + page.Substring(index + 9);
            }

            return page;
        }

        static void OutPutResponse(HttpListenerResponse response, string responseString)
        {
            var responseBytes = System.Text.Encoding.UTF8.GetBytes(responseString);
            OutPutByteResponse(response, System.Text.Encoding.UTF8.GetBytes(responseString));
        }
        static void OutPutByteResponse(HttpListenerResponse response, Byte[] responseBytes)
        {
            response.ContentLength64 = responseBytes.Length;
            response.OutputStream.Write(responseBytes, 0, responseBytes.Length);

        }

        static Hashtable ParseParamsFromStream(Stream inputStream)
        {
            byte[] buffer = new byte[inputStream.Length];
            inputStream.Read(buffer, 0, (int)inputStream.Length);

            return ParseParams(System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length));
        }

        static Hashtable ParseParams(string rawParams)
        {
            Hashtable hash = new Hashtable();

            string[] parPairs = rawParams.Split('&');
            foreach (string pair in parPairs)
            {
                string[] nameValue = pair.Split('=');
                hash.Add(nameValue[0], nameValue[1]);
            }

            return hash;
        }
        static string CreateMainPage(string message)
        {

            return "<!DOCTYPE html><html><body>" +
                    "<h1>NanoFramework</h1>" +
                    "<form method='POST'>" +
                    "<fieldset><legend>Wireless configuration</legend>" +
                    "Ssid:</br><input type='input' name='ssid' value='' ></br>" +
                    "Password:</br><input type='password' name='password' value='' >" +
                    "<br><br>" +
                    "<input type='submit' value='Save'>" +
                    "</fieldset>" +
                    "<b>" + message + "</b>" +
                    "</form></body></html>";
        }
    }
}