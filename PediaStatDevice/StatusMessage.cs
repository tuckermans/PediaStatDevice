using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class StatusMessage
    {
        public UInt16 MeterState { get; private set; }
        public UInt16 CalSignature { get; private set; }
        public UInt32 LastAssayTimestamp { get; private set; }
        
        public DateTime LastAssayTime
        {
            get
            {
                return UnixTime.FromUnixTime((int)LastAssayTimestamp);
            }
        }

        public StatusMessage(byte[] data)
        {
            MeterState = BitConverter.ToUInt16(data, 0);
            CalSignature = BitConverter.ToUInt16(data, 2);
            LastAssayTimestamp = BitConverter.ToUInt32(data, 4);
        }

        public static bool IsBitSet(ushort mask, ushort value)
        {
            return ((mask & value) == value);
        }
    }
}
