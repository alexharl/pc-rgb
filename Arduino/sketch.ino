#include "FastLED.h"

#define BAUDRATE 115200

// number of LEDs on compoments
int8_t NUM_LEDS[] = {12, 8, 8, 6, 6, 20};
#define NUM_COMPONENTS 6 // always sizeof(NUM_LEDS)
#define MAX_LEDS 20      // max number of leds a component can have

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

// LED arrays for FastLED
CRGB leds[NUM_COMPONENTS][MAX_LEDS];

enum commandType_t
{
    SET_COMPONENT = 1,
    SHOW = 2
};

void setup()
{
    Serial.begin(BAUDRATE);

    FastLED.addLeds<WS2811, GPU_PIN, RGB>(leds[GPU], NUM_LEDS[GPU]);
    FastLED.addLeds<WS2811, RAM_1_PIN, RGB>(leds[RAM_1], NUM_LEDS[RAM_1]);
    FastLED.addLeds<WS2811, RAM_2_PIN, RGB>(leds[RAM_2], NUM_LEDS[RAM_2]);
    FastLED.addLeds<WS2811, RESERVOIRE_PIN, RGB>(leds[RESERVOIRE], NUM_LEDS[RESERVOIRE]);
    FastLED.addLeds<WS2811, SSD_PIN, RGB>(leds[SSD], NUM_LEDS[SSD]);
    FastLED.addLeds<WS2811, CPU_PIN, RGB>(leds[CPU], NUM_LEDS[CPU]);
}

void waitForSerial()
{
    while (!Serial.available())
    {
    }
}

void loop()
{
    waitForSerial();

    uint8_t command = Serial.read();
    switch (command)
    {
    case SET_COMPONENT:
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

    // read component index
    uint8_t component_index = Serial.read();
    if (component_index >= NUM_COMPONENTS)
        return;

    int led_index = 0;
    while (led_index < NUM_LEDS[component_index])
    {
        uint8_t hsv[3] = {0, 0, 0};

        waitForSerial();
        Serial.readBytes((uint8_t *)hsv, 3);

        leds[component_index][led_index] = CHSV(hsv[0], hsv[1], hsv[2]);

        led_index++;
    }
}