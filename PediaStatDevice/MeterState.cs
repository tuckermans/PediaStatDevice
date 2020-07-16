using System;

namespace PediaStatDevice
{
    public enum MeterStateEnum: byte
    {
        NOT_CAL,        // not calibrated - won't leave this state until meter is calibrated
        READY,          // ready to read a button or act on inserted sensor
        WAIT_SAMPLE,    // sensor inserted -looking for sample, new cal, or timeout
        TESTING,        // sample aquired - now testing
        SHOW_RESULT,    // display good result to operator
        ERROR,          // error shown and logged
        Disconnected,
        Unknown
    }
}
