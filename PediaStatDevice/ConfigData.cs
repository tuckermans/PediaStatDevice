using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{

    public class ConfigData
    {
        static private ushort _MMOL_DL_BITS  = 0x0001;
        static private ushort _TONES_BIT = 0x0002;      // Audible tone setting
        static private ushort _PID_BIT = 0x0008;      // Patient ID required
        static private ushort _PWD_BIT = 0x0010;      // Password Required

        UInt16 crc;
        public ushort settings;

        public bool Tones
        {
            get
            {
                return SerialMessage.IsBitSet(settings, _TONES_BIT);
            }
            set
            {

            }
        }

        public bool Security
        {
            get
            {
                return SerialMessage.IsBitSet(settings, _PWD_BIT);
            }
            set
            {
                if (value == false)
                {
                    SerialMessage.ClearBit(settings, _PWD_BIT);
                }
                else
                {
                    SerialMessage.SetBit(settings, _PWD_BIT);
                }

            }
        }

        public bool PatientID
        {
            get
            {
                return SerialMessage.IsBitSet(settings, _PID_BIT);
            }
            set
            {
                if (value == false)
                {
                    SerialMessage.ClearBit(settings, _PID_BIT);
                }
                else
                {
                    SerialMessage.SetBit(settings, _PID_BIT);
                }
            }
        }

        public bool G_DL
        {
            get
            {
                return (false == SerialMessage.IsBitSet(settings, _MMOL_DL_BITS));
            }
            set
            {
                if (value == false)
                {
                    SerialMessage.ClearBit(settings, _MMOL_DL_BITS);
                }
                else
                {
                    SerialMessage.SetBit(settings, _MMOL_DL_BITS);
                }
            }
        }

        public ConfigData(byte[] data)
        {
            crc = (UInt16)SerialMessage.PackWord(data, 0);
            settings = (ushort)SerialMessage.PackWord(data, 2);
        }
    }
}
