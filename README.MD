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
Maximum Framerate depends on this value. Currently this program only works for a baudrate <= `19200`

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

# Arduino

LED color values are sent via serial connection.  
Edit `sketch.ino` to fit your components.

```cpp
// number of LEDs on compoments
int8_t NUM_LEDS[] = {12, 8, 8, 6, 6, 20};
#define NUM_COMPONENTS 6 // always sizeof(NUM_LEDS)
#define MAX_LEDS 20 // max number of leds a component can have

// indexes of components (for readability)
#define GPU 0
#define RAM_1 1
#define RAM_2 2
#define RESERVOIRE 3
#define SSD 4
#define CPU 5

// pins of components (where are the components connected)
#define GPU_PIN 48
#define RAM_1_PIN 49
#define RAM_2_PIN 50
#define RESERVOIRE_PIN 51
#define SSD_PIN 52
#define CPU_PIN 53
```

First byte received specifies the command:

- 1 = SET_COMPONENT

## SET_COMPONENT

Second byte specifies index of component.  
Number of expected bytes is calculated from `NUM_LEDS[index]` times `3` (for Hue, Saturation und Brightness)    
```
SET_COMPONENT
|
|  2 => RAM_2
|  |
|  |   ___________ NUM_LEDS[RAM_2] x 3 = 8 x 3 = 24 ______________
|  |  |                                                           |
1  2  123 64 211 12 233 66 . . . . . . . . . . . . . . . .  123 54
      |   |   |  |       |
      |   |   |   -------
      |   |   |    LED 2
      |   |   |      
      |   |   |
      |   |   - Brightness
      |   ----- Saturation
      --------- Hue
```

All bytes for a component are read and color values are stored in CRGB leds array. After all bytes for a component were read, `FastLED.show()` ist called.
