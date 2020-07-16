using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PediaStatDevice
{
    /*
        typedef struct {
    INT32  time;                       // in time.h format (MUST be first field)
    UINT8   eventID;                    // event ID, assay, error, etc.
    CHAR    lotCode[LOTCODE_SIZE - 1];  // exclude terminating NULL in string storage
    UINT16  result;                     // assay result or extended error info
    UINT16  crc;                        // record CRC (MUST be last field in struct)
} EventType;
     */
    public enum EventLogID : byte
    {
    }
    public class MeterEventLog
    {
        private static readonly int BIN_SIZE = 14;

        private ushort _crc;
        public ushort CRC
        {
            get { return _crc; }
        }

        private DateTime _date = DateTime.MinValue;
        public DateTime Date
        {
            get { return _date; }
        }

        private Int32 _unixDate = -1;
        public Int32 UnixDate
        {
            get { return _unixDate; }
        }

        private byte _eventID;
        public byte EventID
        {
            get { return _eventID; }
        }

        public string EventType
        {
            get
            {
                switch (_eventID)
                {
                    case 1:
                        return "Assay";
                    case 2:
                        return "Error";
                    case 3:
                        return "Post";
                    case 4: 
                        return "Cal";
                    case 6:
                        return "Control";
                    default:
                        return "Unknown";
                }                    
            }
        }
        private string _lotCode = string.Empty;
        public string LotCode
        {
            get { return _lotCode; }
        }

        private ushort _result;
        public ushort Result
        {
            get { return _result; }
        }

        public MeterEventLog(Int32 unixDate, byte eventID, string lotCode, ushort result, ushort crc)
        {
            _unixDate = unixDate;
            if (_unixDate != -1)
            {
                _date = UnixTime.FromUnixTime(_unixDate);
            }
            _eventID = eventID;
            _lotCode = lotCode;
            _result = result;
            _crc = crc;
        }

        public static MeterEventLog FromBinary(byte[] rawData)
        {
            if (rawData.Length < BIN_SIZE)
            {
                throw new InvalidDataException("Binary data is too small");
            }
            int offset = 0;
            Int32 unixTime = BitConverter.ToInt32(rawData, offset);
            offset += sizeof(Int32);
            byte evid = rawData[offset++];

            //string lot = Encoding.ASCII.GetString(rawData, offset, 5);
            string lot = StringUtils.DecodeASCIIBytes(rawData, offset, 5);
            offset += 5;
            ushort result = BitConverter.ToUInt16(rawData, offset);
            offset += sizeof(ushort);

            ushort crc = BitConverter.ToUInt16(rawData, offset);
            
            return new MeterEventLog(unixTime, evid, lot, result, crc);
        }
    }
}
