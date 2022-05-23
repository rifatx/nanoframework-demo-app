## Summary
Demo application written in C# to be run on an [ESP32](https://www.espressif.com/en/products/socs/esp32) with [nanoFramework](https://www.nanoframework.net/).

## Features
- A simple web server using [WebServer](https://github.com/nanoframework/nanoFramework.WebServer) library.
- A led controller class to blink arbitrary patterns.
- Ability to serve a basic settings page for wifi connection settings.
- Simple webapi controller to send commands to data pins (for instance, to a relay).
- A health check endpoint with visual feedback.