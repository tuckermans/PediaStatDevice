using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    [Flags]
    public enum SampleResultStatus: byte
    {
        SampleIDPresent = (byte)0x10,
        ResultsPending = (byte)0x80
    }


    public class SampleResult
    {
        private const int SAMPLE_ID_LEN = 21; // 20 + null terminator
        private const int LOTCODE_LEN = 6;    // 5 + null terminator

        public static readonly uint HIGH_LEAD = 0xFFFF;
        public static readonly uint LOW_LEAD = 0xFFFE;


        public bool Valid { get; private set; }
        public ErrorCode Error { get; private set; }
        public byte Status { get; private set; }
        public ushort PendingTimeLeft { get; private set; }
        public ushort CalSignature { get; private set; }
        public float TempBeforeDep { get; private set; }
        public float TempAfterDep { get; private set; }
        public float TempAfterAssay { get; private set; }
        public float TempAvg { get; private set; }
        public ushort LowerMinimum { get; private set; }
        public ushort UpperMinimum { get; private set; }
        private short Slope;
        private ushort SWCscaled;

        public DateTime Timestamp { get; private set; }
        private uint SWCValue;
        private uint TempCorrected;
        private uint BLLCorrected;
        public float Result { get; set; }

        public bool Transmitted { get; private set; }
        public string SampleID { get; set; }
        public string LotCode { get; private set; }

        public SampleResult(byte[] data)
        {
            Int16 int16Value = 0;
            UInt32 uint32value = 0;
            Int32 int32value = 0;
            int idx = 0;

            Valid = BitConverter.ToBoolean(data, idx);
            idx++;

            Error = (ErrorCode)data[idx++];
            CalSignature = BitConverter.ToUInt16(data, idx);
            idx += sizeof(UInt16);

            Status = data[idx++];

            PendingTimeLeft = BitConverter.ToUInt16(data, idx);
            idx += sizeof(UInt16);

            int16Value = BitConverter.ToInt16(data, idx);
            TempBeforeDep = (float)int16Value / 10.0f;
            idx += sizeof(Int16);

            int16Value = BitConverter.ToInt16(data, idx);
            TempAfterDep = (float)int16Value / 10.0f;
            idx += sizeof(Int16);

            int16Value = BitConverter.ToInt16(data, idx);
            TempAfterAssay = (float)int16Value / 10.0f;
            idx += sizeof(Int16);

            int16Value = BitConverter.ToInt16(data, idx);
            TempAvg = (float)int16Value / 10.0f;
            idx += sizeof(Int16);

            LowerMinimum = BitConverter.ToUInt16(data, idx);
            idx += sizeof(UInt16);

            UpperMinimum = BitConverter.ToUInt16(data, idx);
            idx += sizeof(UInt16);

            Slope = BitConverter.ToInt16(data, idx);
            idx += sizeof(Int16);

            SWCscaled = BitConverter.ToUInt16(data, idx);
            idx += sizeof(UInt16);

            int32value = BitConverter.ToInt32(data, idx);
            Timestamp = UnixTime.FromUnixTime(int32value);
            idx += sizeof(Int32);

            SWCValue = BitConverter.ToUInt32(data, idx);
            idx += sizeof(UInt32);

            TempCorrected = BitConverter.ToUInt32(data, idx);
            idx += sizeof(UInt32);

            BLLCorrected = BitConverter.ToUInt32(data, idx);
            idx += sizeof(UInt32);


            uint32value = BitConverter.ToUInt32(data, idx);
            if (uint32value != LOW_LEAD && uint32value != HIGH_LEAD)
            {
                Result = (float)uint32value / (float)10.0;
            }
            else
            {
                Result = uint32value;
            }

            idx += sizeof(UInt32);

            Transmitted = BitConverter.ToBoolean(data, idx);
            idx++;

            int nullIdx = FindFirstNullByte(data, idx, SAMPLE_ID_LEN);
            if (nullIdx > 0)
            {
                SampleID = Encoding.ASCII.GetString(data, idx, nullIdx);
            }
            idx += SAMPLE_ID_LEN;

            LotCode = Encoding.ASCII.GetString(data, idx, LOTCODE_LEN-1);
        }

        /// <summary>
        /// Find the first occurrence of a null byte in the array.
        /// Used to circumvent issue with ASCII to string decoding of fixed buffers
        /// </summary>
        /// <param name="data">array to examine</param>
        /// <param name="idx">starting position</param>
        /// <param name="len">length to examine</param>
        /// <returns>index of first null byte, or -1 if one was not found</returns>
        private int FindFirstNullByte(byte[] data, int idx, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (data[idx + i] == (byte)0)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
