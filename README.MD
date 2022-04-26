# Prerequisites

## Arduino
If you are using VSCode:   
- [VSCode Arduino Extension](https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.vscode-arduino)  
- [C/C++ Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.cpptools)

## Backend

Download and install [.NET Core SDK](https://dotnet.microsoft.com/en-us/download)

## WebClient

`yarn` has to be installed on your local machine

# Setup

## Install .NET dependencies

```cmd
> cd .\Backend
> dotnet restore
```

## Install WebClient dependencies

```cmd
> cd .\WebClient
> yarn
```

## Environment

### PCRGB\_\_ComPortName

Serial port name where the Arduino is connected.

### PCRGB\_\_ComPortBaudrate (optional)

Baudrate to use for serial communication.  
Maximum Framerate depends on this value.

Default: `115200`

### PCRGB\_\_ControllerConfig

Path to config.  
Example config `renderer_config.json` can be found in project root folder.

# Configuration

The renderer configuration has to have a `name`, `width` and `height`. You can also specify an array of `controllers`.

```json
{
  "name": "Canvas", // name of layer (only for readability)
  "width": 20, // layer width
  "height": 20, // layer height
  "controllers": []
}
```

## Controller

Like renderer configuration each controller must have a `name`, `width` and `height`. Also you have to specify this layers position via `x` and `y`.  
The `id` proptery is used a the controller index in FastLED.

```json
{
  "id": 1, // controller index in FastLED
  "x": 6, // global x position
  "y": 4, // global y position
  "width": 1, // layer width
  "height": 15, // layer height
  "name": "Ram 1", // name of controller (only for readability)
  "pixelPositions": []
}
```

`pixelPositions` contains a list of positions for the LEDs relative to the contoller layer.

```json
{ "x": 0, "y": 14 }
```

# Start

## Server

If you are using VSCode you can just create a `launch.json` and start by pressing `F5`.

## WebClient

```cmd
> cd .\WebClient
> yarn start
```

Open http://localhost:3000 in your browser.

You can also build the WebClient so you dont have to have a react dev build running

```cmd
> cd .\WebClient
> yarn build
```

then copy the contents of folder `WebClient/build` to `Backend/wwwroot`  
You can now access the WebClient via http://localhost:5000 in your browser.

## Arduino

[ReadMe](/Arduino/README.MD)
