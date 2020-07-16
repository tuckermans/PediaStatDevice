using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class TimeMessage
    {
        public DateTime Time
        {
            get
            {
                return UnixTime.FromUnixTime(RtcTime);
            }
        }

        public Int32 RtcTime { get; private set; }

        byte[] _rtcChipData = new byte[6];
        public byte[] RtcChipTime
        {
            get
            {
                return _rtcChipData;
            }
        }


        public TimeMessage(byte[] data)
        {
            RtcTime = BitConverter.ToInt32(data, 0);
        }

        public TimeMessage(DateTime dt)
        {
            RtcTime = UnixTime.ToUnixTime(dt);
            UpdateChipData(dt);
        }

        private void UpdateChipData(DateTime dt)
        {
            _rtcChipData[0] = (byte)(dt.Year - 2000);
            _rtcChipData[1] = (byte)dt.Month;
            _rtcChipData[2] = (byte)dt.Day;
            _rtcChipData[3] = (byte)dt.Hour;
            _rtcChipData[4] = (byte)dt.Minute;
            _rtcChipData[5] = (byte)dt.Second;
        }
    }
}
