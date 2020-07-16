using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public enum SwitchID
    {
        ENABLE_DUMMY_LOAD = 0,
        ENABLE_CHAN1_SENSOR,
        ENABLE_CHAN2_SENSOR,
        ENABLE_DUAL_POT,
        ENABLE_FEEDBACK,
        ENABLE_PEAKGEN_RESET,
        ENABLE_PEAKGEN_CLOCK,
        ENABLE_HIGH_SPEED_OPTICAL,
        ENABLE_OPTICAL_LEDS,
    };

    // Definition Of All Optical Led's
    public enum OpticalLedID
    {
        LED0 = 0,
        LED1,
        LED2,
        LED3,
        LED4,
        LED5,
        LED6,
        LED7,
    };

    public enum DacAddress
    {
        DAC_ADDR_A,
        DAC_ADDR_B,
        DAC_ADDR_O,
        DAC_ADDR_POT2,
        DAC_ADDR_ALL = 0x0F,
    };

    public enum SensorIDEnum
    {
        LEAD_SENSOR_ID = 0,
        HCT_SENSOR_ID,
        BILLIRUBIN_SENSOR_ID,
        OPTICAL_VERIFIER_ID,
        CHOLESTEROL_SENSOR_ID,
        SPARE5_SENSOR_ID,
        SPARE6_SENSOR_ID,
        SPARE7_SENSOR_ID,
        SPARE8_SENSOR_ID,
        SPARE9_SENSOR_ID,
        SPARE10_SENSOR_ID,
        SPARE11_SENSOR_ID,
        LEAD_HGB_SENSOR_ID,
        SPARE13_SENSOR_ID,
        SPARE14_SENSOR_ID,
        ELECTRONIC_SENSOR_ID,
    };

    public class ADCData
    {
        public short VDAC2;
        public short TEMP;
        public short OPTICAL;
        public short IMEAS2;
        public short FBK1;
        public short VDAC1;
        public short IMEAS1;
        public short FBK2;
        public short VDRIVE1;
        public short OPTICAL_DIAG;
        public short STRAY;
        public short EMPTY;
        public short FILLED;

        public ADCData(byte[] Data)
        {
            VDAC2 = SerialMessage.PackWord(Data, 0);
            TEMP = SerialMessage.PackWord(Data, 2);
            OPTICAL = SerialMessage.PackWord(Data, 4);
            IMEAS2 = SerialMessage.PackWord(Data, 6);
            FBK1 = SerialMessage.PackWord(Data, 8);
            VDAC1 = SerialMessage.PackWord(Data, 10);
            IMEAS1 = SerialMessage.PackWord(Data, 12);
            FBK2 = SerialMessage.PackWord(Data, 14);
            VDRIVE1 = SerialMessage.PackWord(Data, 16);
            OPTICAL_DIAG = SerialMessage.PackWord(Data, 18);
            STRAY = SerialMessage.PackWord(Data, 20);
            EMPTY = SerialMessage.PackWord(Data, 22);
            FILLED = SerialMessage.PackWord(Data, 24);
        }

    };

    public class LEDStatusType
    {
        public short State
        {
            get;
            set;

        }

        public OpticalLedID Color
        {
            get;
            set;
        }

        public short Current
        {
            get;
            set;
        }

        public LEDStatusType(byte[] Data)
        {
            State = Data[26];
            Color = (OpticalLedID)Data[27];
            Current = SerialMessage.PackWord(Data, 28);
        }

    };
    public class AnalyzerDataEvent : EventArgs
    {
        protected LcDataPacket _data;
        public AnalyzerDataEvent(LcDataPacket packet)
        {
            _data = packet;
        }
        public AnalyzerDataEvent()
        {
        }
    }

    public class SwitchStatusEvent : AnalyzerDataEvent
    {
        public SwitchID _sw;
        public short _status;

        public SwitchStatusEvent(LcDataPacket packet) : base(packet)
        {
            if (null != packet)
            {
                _sw = (SwitchID)packet.Data[0];
                _status = SerialMessage.PackWord(packet.Data, 1);
            }
        }
    }

    public class AnalogDataStatusEvent : AnalyzerDataEvent
    {
        public ADCData AnalogData;

        public LEDStatusType LEDStatus;

        public AnalogDataStatusEvent(LcDataPacket packet) :base(packet)
        {
            if (null != packet)
            {
                AnalogData = new ADCData(packet.Data);
                LEDStatus = new LEDStatusType(packet.Data);
            }
        }
        public AnalogDataStatusEvent(byte[] Data)
        {
            AnalogData = new ADCData(Data);

            LEDStatus = new LEDStatusType(Data);
        }

    }

    public class ChannelGainStatusEvent : AnalyzerDataEvent
    {
        public int Channel = 0;
        public short _gain;

        public ChannelGainStatusEvent(LcDataPacket packet) : base(packet)
        {
            Channel = packet.Data[0];
            _gain = packet.Data[1];
        }
    }

    public class SensorStateChangeEvent : AnalyzerDataEvent
    {
        public bool _state = false;
        public SensorIDEnum _id;

        public SensorStateChangeEvent(LcDataPacket packet) : base(packet)
        {
            byte status = packet.Data[0];
            _state = (status == 1) ? true : false;
            if (_state == true)
            {
                _id = (SensorIDEnum)packet.Data[1];
            }
        }
    }

  

}
