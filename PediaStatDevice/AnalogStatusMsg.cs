using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class AnalogStatusMsg
    {
        short[] _status = null;
        byte[] _payload = null;
        int _numChannels = 0;
        int _numBytes = 0;

        public AnalogStatusMsg(short[] status, int NumChannels)
        {
            _status = new short[NumChannels];
            _numChannels = NumChannels;

            Buffer.BlockCopy(status, 0, _status, 0, _numChannels);
            Marshall();
        }

        public AnalogStatusMsg(byte[] payload, int NumBytes)
        {
            _payload = new byte[NumBytes];
            _numBytes = NumBytes;
            Buffer.BlockCopy(payload, 0, _payload, 0, NumBytes);
            UnMarshall();
        }


        // Convert Analog data to send 
        private void Marshall()
        {
            short idx = 0;

            // Need 2 bytes for each channel
            _payload = new byte[_numChannels * 2];

            foreach (short ch in _status)
            {
                SerialMessage.SplitWord(ch, ref _payload, idx);
                idx += 2;
            }
        }

        // Convert byte to Analog Status
        private void UnMarshall()
        {
            for (int i = 0, idx = 0; i < _numBytes; i += 2, idx++)
            {
                _status[idx] = SerialMessage.PackWord(_payload, i + 2);
            }

        }
        public short[] Status
        {
            get
            {
                return _status;
            }
        }
        public byte[] Payload
        {
            get
            {
                return _payload;
            }
        }
    }
}
