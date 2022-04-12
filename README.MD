# Prerequisites

Download and install [.NET Core SDK](https://dotnet.microsoft.com/en-us/download)  
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
Default: `19200`

### PCRGB\_\_ComponentsConfig

Path to config.  
Example config `renderer_config.json` can be found in project root folder.

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

N/A
