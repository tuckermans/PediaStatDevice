using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class EventLog
    {
        public Int32 time { get; set; }                          // in time.h format (MUST be first field)
        public Int16 timeOffsetMins {get;set;}                // Local Time Offset  
        public ushort eventID {get;set;}                        // event ID, assay, error, etc.
        public string parameter {get; set;}                    // exclude terminating NULL in string storage
        public UInt16 result {get;set;}                     // assay result or extended error info
        public UInt16 crc;                        // record CRC (MUST be last field in struct)

        private string[] EventStrings = { "NULL", "PB Assay", "Error", "POST", "Cal","Assert",
                                        "HGB Assay", "HCT Assay","Logon", "Logoff",
                                        "Time Changed", "Date Changed", "PB QC", "HGB QC",
                                        "Bili Assay", "Bili QC", "OverCurrent",
                                        "Cholesterol Assay", "Cholesterol QC",
                                        "HCT QC"};
        
        public string EventStr
        {
            get
            {
                if (eventID < EventStrings.Length)
                {
                    return EventStrings[eventID];
                }
                return eventID.ToString();
            }
        }

        static public string Header
        {
            get
            {
                return "Time,Event ID,Result,Parameter";
            }
        }
        public string ToCSV
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(Timestamp).Append(",")
                    .Append(eventID).Append(",")
                    .Append(result).Append(",")
                    .Append(parameter);

                return builder.ToString();
            }
        }

        public string Timestamp
        {
            get
            {
                string temp;
                temp = UnixTime.FromUnixTime(time).ToString("yyyy-MM-ddTHH:mm:ss");

                int offsetMins, offsetSecs;
                offsetMins = Math.Abs(timeOffsetMins / 60);
                offsetSecs = Math.Abs(timeOffsetMins % 60);

                if( timeOffsetMins >= 0)
                    return string.Format("{0}+{1:D2}:{2:D2}", temp, offsetMins, offsetSecs);
                else
                    return string.Format("{0}-{1:D2}:{2:D2}", temp, offsetMins, offsetSecs);
            }
        }

        public EventLog()
        {
        }
        public EventLog(byte[] data)
        {
            int idx = 0;
            time = (Int32)SerialMessage.PackDWord(data, idx);
            idx += 4;

            timeOffsetMins = (Int16)SerialMessage.PackWord(data, idx);
            idx += 2;

            eventID = data[idx];
            idx += 1;
            
            parameter = Encoding.ASCII.GetString(data, idx, 21);
            idx += 21;
            
            result = (UInt16)SerialMessage.PackWord(data, idx);            
            idx += 2;
            
            crc = (UInt16)SerialMessage.PackWord(data, idx);
        }

    }

}
