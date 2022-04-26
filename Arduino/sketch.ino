#include "FastLED.h"
#include "FastLEDStream.h"

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

int8_t NUM_LEDS[] = {12, 8, 8, 6, 6, 20}; // number of LEDs on compoments
#define NUM_COMPONENTS 6                  // always sizeof(NUM_LEDS)
#define MAX_LEDS 20                       // max number of leds a component can have

// CRGB buffer for FastLED
CRGB leds[NUM_COMPONENTS][MAX_LEDS];

void setup()
{
    // recommended baudrate
    Serial.begin(115200);

    // initialize FastLED controllers
    // see: https://github.com/FastLED/FastLED/wiki/Overview
    FastLED.addLeds<WS2811, GPU_PIN, BRG>(leds[GPU], NUM_LEDS[GPU]);
    FastLED.addLeds<WS2811, RAM_1_PIN, BRG>(leds[RAM_1], NUM_LEDS[RAM_1]);
    FastLED.addLeds<WS2811, RAM_2_PIN, BRG>(leds[RAM_2], NUM_LEDS[RAM_2]);
    FastLED.addLeds<WS2811, RESERVOIRE_PIN, BRG>(leds[RESERVOIRE], NUM_LEDS[RESERVOIRE]);
    FastLED.addLeds<WS2811, SSD_PIN, BRG>(leds[SSD], NUM_LEDS[SSD]);
    FastLED.addLeds<WS2811, CPU_PIN, BRG>(leds[CPU], NUM_LEDS[CPU]);
}

void loop()
{
    FastLEDStream::command(Serial);
}