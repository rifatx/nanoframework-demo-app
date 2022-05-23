﻿namespace NFTest1.Wifi
{
    internal class Constants
    {
        internal const string MAIN_HTML = @"<!DOCTYPE html>
<html>
 <body>
  <h1>NanoFramework</h1>
  <form method='POST'>
    <fieldset><legend>Wireless configuration</legend>
        Ssid:<br><input type=""text"" name=""ssid"" value=""""><br>
        Password:<br><input type=""text"" name=""password"" value=""""><br>
        <br>
        <input type=""submit"" value=""Save"">
    </fieldset>
  <b>{message}</b>
  </form>
 </body>
</html>";
    }
}



