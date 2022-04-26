#include "FastLEDStream.h"
#include "FastLED.h"

/**
 * wait until a new byte is available
 *
 * @param port Serial port to check
 */
static void FastLEDStream::waitForData(Stream &port)
{
    while (!port.available())
    {
    }
}

/**
 * check for command and execute
 *
 * @param port Serial port to read from
 */
static void FastLEDStream::command(Stream &port)
{
    waitForData(port);

    // interpret first byte as command
    switch (port.read())
    {
    case set_controller:
        waitForData(port);
        // second byte as controller index
        streamToController(port.read(), port);
        break;
    case show:
        FastLED.show();
        break;
    }
}

/**
 * stream incoming bytes into controller LEDs
 *
 * @param controller Index of controller to read into
 * @param port Serial port to read from
 */
static void FastLEDStream::streamToController(uint8_t controller, Stream &port)
{
    // check wether controller index is valid
    if (controller >= FastLED.count())
        return;

    uint8_t num_leds = FastLED[controller].size();

    for (uint8_t led = 0; led < num_leds; led++)
    {
        uint8_t buffer[3];

        waitForData(port);
        port.readBytes((uint8_t *)buffer, sizeof(buffer));

        // CHSV converts those 3 bytes to CRGB values
        FastLED[controller][led] = (CHSV &)buffer;
    }
}
