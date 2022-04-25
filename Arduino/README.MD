# General

By sending data via serial connection the `FastLED` library can be controlled.

# Setup

The only thing needed is to call `FastLEDSerial()` in every loop so the commands and color values can be received via serial.  
Minimalist setup:

```cpp
#include "FastLED.h"
#include "FastLEDSerial.h"
CRGB leds[8]; // 8 LEDs
void setup()
{
    Serial.begin(115200);
    FastLED.addLeds<WS2811, 2>(leds, 8); // PIN 2
}
void loop()
{
    FastLEDSerial(); // listen for commands
}
```

# Commands

Every first byte received is interpreted as a command:

- `1` = SET_CONTROLLER
- `2` = SHOW

## SET_CONTROLLER

Second byte specifies index of controller.  
Number of expected bytes for color values equals to the number of LEDs times `3` (for Hue, Saturation und Brightness)

### Example

```
SET_CONTROLLER
|
|  Controller at index 2
|  |
|  |   ___________ FastLED[2].size() x 3 _________________________
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

All bytes for a component are read and color values are stored in CRGB leds array.

## SHOW

If the first byte transmitted is `2` `FastLED.show()` ist called.
