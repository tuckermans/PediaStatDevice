using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class SerialMessage
    {
        public byte[] Payload = new byte[128];
        public uint _length;
        private byte[] _payload;
        public CmdIDType _cmd;
        protected int Index = 0;

        public static bool IsLevel(ulong mask, ushort value)
        {
            return ((mask & Constants.LEVEL_BIT_MASK) == value);
        }

        public static bool IsSensorID(ulong mask, ushort value)
        {
            return (((mask & Constants.SENSOR_ID_MASK) >> 8) == value);
        }
        
        public static bool IsBitSet(ulong mask, ushort value)
        {
            return ((mask & value) == value);
        }

        public static void SetBit(ulong mask, ushort value)
        {
            mask |= value;
        }

        public static int ClearBit(ulong mask, ushort value)
        {
            int settings = 26, result = 0;
            int bit = 0x0001;

            result = (settings & ~bit);

            mask &= (ushort)~value;

            return result;
        }

        public static short PackWord(byte[] buff, int start)
        {
            return (short)((buff[start + 1] << 8) | buff[start]);
        }

        public static UInt32 PackDWord(byte[] buff, int start)
        {
            return (UInt32)((buff[start + 3] << 24) | (buff[start + 2] << 16) | (buff[start + 1] << 8) | buff[start]);
        }

        /// <summary>
        /// Pack 4 bytes into a float
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static float GetFloat(byte[] payload, int start)
        {
            unsafe
            {
                float a;

                byte* pBuff = (byte*)&a;
                *pBuff++ = payload[start + 0];
                *pBuff++ = payload[start + 1];
                *pBuff++ = payload[start + 2];
                *pBuff = payload[start + 3];

                return a;
            }

        }

        public static void SplitWord(short value, ref byte[] Payload, short  start)
        {
            Payload[start] = (byte)(value & 0x00FF);
            Payload[start+1] = (byte)((value >> 8) & 0x00FF);
        }

        public static void SplitDWord(UInt32 value, ref byte[] Payload, UInt16 start)
        {
            Payload[start] = (byte)(value & 0x00FF);
            Payload[start + 1] = (byte)((value >> 8) & 0x00FF);
            Payload[start + 2] = (byte)((value >> 16) & 0x00FF);
            Payload[start + 3] = (byte)((value >> 24) & 0x00FF);

        }

        public SerialMessage(CmdIDType cmd, byte[] payload, uint len)
        {
            _cmd = cmd;
            _length = len;
            _payload = new byte[len];

            Buffer.BlockCopy(payload, 0, _payload, 0, (int)_length);
        }

        public SerialMessage(CmdIDType cmd)
        {
            _cmd = cmd;
            _length = 1;
            _payload = new byte[_length];
            _payload[0] = (byte)_cmd;

        }

        public string GetString(int offset, int length)
        {
            string str = System.Text.Encoding.ASCII.GetString(_payload, Index + offset, length);
            Index += length;
            return str;
        }
        public Int16 GetInt16()
        {
            return (Int16)GetWord();
        }
        public short GetShort()
        {
            return (short)GetWord();
        }
        public UInt16 GetUInt16()
        {
            return (UInt16)GetWord();
        }
        public short GetWord()
        {
            short val = (short)((_payload[Index + 1] << 8) | _payload[Index]);
            Index += 2;
            return val;
        }
        public Byte GetByte()
        {
            byte b = _payload[Index];
            Index += 1;
            return b;

        }

        public UInt32 GetUInt32()
        {
            return (UInt32)GetDWord();
        }
        public Int32 GetInt32()
        {
            return (Int32)GetDWord();
        }
        public UInt32 GetDWord()
        {
            UInt32 val = (UInt32)((_payload[Index + 3] << 24) | (_payload[Index + 2] << 16) | (_payload[Index + 1] << 8) | _payload[Index]);
            Index += 4;
            return val;
        }
        public void Skip(int count)
        {
            Index += count;
        }

        public SerialMessage()
        {
            Index = 0;
        }

    };

}
