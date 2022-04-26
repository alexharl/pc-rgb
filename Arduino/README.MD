# General

If you are using `FastLED` in your Arduino project you can use this library to read `CHSV` color values from a stream into your `CRGB` led array.

# Setup

The only thing needed is to call `FastLEDStream::command(stream)` to read the commands from your stream.  
You can pass any `Stream` to the `command` function e.g. `Serial`.

**Minimalist setup:**

```ino
#include "FastLED.h"
#include "FastLEDStream.h"

CRGB leds[8]; // 8 LEDs

void setup()
{
    Serial.begin(115200);
    FastLED.addLeds<WS2811, 2>(leds, 8); // Digital Pin 2
}

void loop()
{
    FastLEDStream::command(Serial); // listen for commands
}
```

# Commands

Every first byte received is interpreted as a command:

- `1` = set_controller
- `2` = show

## set_controller

Second byte specifies index of controller.  
Number of expected bytes for color values equals to the number of LEDs times `3` (for Hue, Saturation und Brightness)

### Example

```
set_controller
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

## show

If the first byte transmitted is `2` `FastLED.show()` ist called.
