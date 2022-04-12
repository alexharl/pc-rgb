#include "FastLED.h"

// indexes of components
#define GPU 0
#define RAM_1 1
#define RAM_2 2
#define RESERVOIRE 3
#define SSD 4
#define CPU 5

// pins of components
#define GPU_PIN 48
#define RAM_1_PIN 49
#define RAM_2_PIN 50
#define RESERVOIRE_PIN 51
#define SSD_PIN 52
#define CPU_PIN 53

#define NUM_COMPONENTS 6
#define MAX_LEDS 20

// number of LEDs on compoments
int8_t NUM_LEDS[] = {12, 8, 8, 6, 6, 20};

// LED arrays for FastLED
CRGB leds[NUM_COMPONENTS][MAX_LEDS];

void setup()
{
    Serial.begin(19200);
    initializeFastLED();
}

void loop()
{
    checkIncommingData();
}

void initializeFastLED()
{
    FastLED.addLeds<WS2811, GPU_PIN, RGB>(leds[GPU], NUM_LEDS[GPU]);
    FastLED.addLeds<WS2811, RAM_1_PIN, RGB>(leds[RAM_1], NUM_LEDS[RAM_1]);
    FastLED.addLeds<WS2811, RAM_2_PIN, RGB>(leds[RAM_2], NUM_LEDS[RAM_2]);
    FastLED.addLeds<WS2811, RESERVOIRE_PIN, RGB>(leds[RESERVOIRE], NUM_LEDS[RESERVOIRE]);
    FastLED.addLeds<WS2811, SSD_PIN, RGB>(leds[SSD], NUM_LEDS[SSD]);
    FastLED.addLeds<WS2811, CPU_PIN, RGB>(leds[CPU], NUM_LEDS[CPU]);

    FastLED.show();
}

/*
    DO NOT EDIT BELOW
    (unless you know what you are doing)
*/

enum commandType_t
{
    WAITING_FOR_COMMAND = -1,
    SET_COMPONENT = 1
};
int8_t current_command = WAITING_FOR_COMMAND;

void checkIncommingData()
{
    // check incoming bytes available
    if (Serial.available() > 0)
    {
        // read incominf byte
        uint8_t value = Serial.read();

        // switch what to do with next byte
        switch (current_command)
        {
        case WAITING_FOR_COMMAND:
            handleNewCommand(value);
            break;
        case SET_COMPONENT:
            handleNextByteSetComponent(value);
            break;
        default:
            endCommand();
        }
    }
}

/* set current command if valid */
bool handleNewCommand(uint8_t value)
{
    if (isCommand(value))
        current_command = value;
}

/* check if byte is a valid command */
bool isCommand(uint8_t value)
{
    return value == SET_COMPONENT;
}

/* reset current command */
void endCommand()
{
    current_command = WAITING_FOR_COMMAND;
}

enum colorByteIndex_t
{
    HUE = 0,
    SATURATION = 1,
    BRIGHTNESS = 2
};

int8_t set_component_index = -1;
int set_component_num_bytes = 0;
int set_component_current_byte = 0;
uint8_t set_component_current_values[3] = {0, 0, 0};

void resetSetComponent()
{
    set_component_current_byte = 0;
    set_component_current_values[HUE] = 0;
    set_component_current_values[SATURATION] = 0;
    set_component_current_values[BRIGHTNESS] = 0;
}

void handleNextByteSetComponent(uint8_t value)
{
    /* first byte of sequence specifies component */
    if (set_component_index == -1)
    {
        // invalid component index
        if (value >= NUM_COMPONENTS)
            return endCommand();

        // first byte is component index
        set_component_index = value;

        // expected num bytes for component
        set_component_num_bytes = NUM_LEDS[set_component_index] * 3;

        resetSetComponent();

        return;
    }

    /* all successive bytes are used to set color of pixels */

    // calculate byte index of color
    uint8_t color_byte_index = set_component_current_byte % 3;
    set_component_current_values[color_byte_index] = value;

    // last byte of pixel
    if (color_byte_index == BRIGHTNESS)
        setComponentLED();

    // next byte
    set_component_current_byte++;

    // last byte reached
    if (set_component_current_byte >= set_component_num_bytes)
        endSetComponent();
}

void endSetComponent()
{
    // reset component index
    set_component_index = -1;

    endCommand();
    FastLED.show();
}

void setComponentLED()
{
    uint8_t led_index = set_component_current_byte / 3.f;
    leds[set_component_index][led_index] =
        CHSV(set_component_current_values[HUE],
             set_component_current_values[SATURATION],
             set_component_current_values[BRIGHTNESS]);
}