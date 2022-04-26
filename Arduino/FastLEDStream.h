#ifndef FASTLEDSTREAM_H
#define FASTLEDSTREAM_H

#include <Arduino.h>

enum FastLEDStreamCommand_t
{
    set_controller = 1,
    show = 2
};

/* Reads serial as commands to control FastLED */
class FastLEDStream
{
public:
    static void command(Stream &port);
    static void streamToController(uint8_t controller, Stream &port);

private:
    static void waitForData(Stream &port);
};

#endif