using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class AdcMessage
    {
        public short[] Channels { get; private set; }

        public AdcMessage(byte[] data)
        {
            Channels = new short[8];
            for (int i = 0; i < 8; i++)
            {
                Channels[i] = BitConverter.ToInt16(data, 2 * i);
            }
        }
    }
}
