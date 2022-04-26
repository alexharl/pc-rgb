#include "FastLEDSerial.h"
#include "FastLED.h"

enum FastLEDSerialCommand_t
{
    SET_CONTROLLER = 1,
    SHOW = 2
};

/* wait until a new byte is available */
void waitForSerial()
{
    while (!Serial.available())
    {
    }
}

void FastLEDSerial()
{
    waitForSerial();

    // interpret first byte as command
    switch (Serial.read())
    {
    case SET_CONTROLLER:
        setComponent();
        break;
    case SHOW:
        FastLED.show();
        break;
    }
}

void setComponent()
{
    waitForSerial();

    // read controller index
    uint8_t controller = Serial.read();

    // check wether controller index is valid
    if (controller >= FastLED.count())
        return;

    uint8_t num_leds = FastLED[controller].size(); // get number of LEDs for controller
    uint8_t buffer[num_leds * 3];                  // prepare buffer

    waitForSerial();
    Serial.readBytes((uint8_t *)buffer, sizeof(buffer));

    // read color values for all LEDs in controller
    for (uint8_t led = 0; led < num_leds; led++)
        FastLED[controller][led] = (CHSV &)buffer[led * 3];
}