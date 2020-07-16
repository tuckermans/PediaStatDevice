using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class QCResult : BaseResult
    {
        public QCMeasuredResults[] Results = new QCMeasuredResults[3];
        public string OID;     // Operator ID (future)
        public int expiration;
        public byte HgbMultApplied;
        public short DiagFlags;

        /// <summary>
        /// UTC fomatted timestamp
        /// </summary>
        public string QCSampleTime
        {
            get
            {
                string temp;
                temp = UnixTime.FromUnixTime(timestamp).ToString("yyyy-MM-ddTHH:mm:ss");

                int offsetMins, offsetSecs;
                offsetMins = Math.Abs(timeOffsetMins / 60);
                offsetSecs = Math.Abs(timeOffsetMins % 60);

                if (timeOffsetMins >= 0)
                    return string.Format("{0}+{1:D2}:{2:D2}", temp, offsetMins, offsetSecs);
                else
                    return string.Format("{0}-{1:D2}:{2:D2}", temp, offsetMins, offsetSecs);
            }
        }

        /// <summary>
        /// UTC Timestamp
        /// </summary>
        public int Timestamp
        {
            get
            {
                return timestamp;
            }
        }

        static public string Header
        {
            get
            {
                return "Time,OID,LotCode,Sample Type,Ch1,Ch1Result,Ch1Target,Ch1Range,Ch1Units,Ch1Flag,Ch2,Ch2Result,Ch2Target,Ch2Range,Ch2Units,Ch2Flag,Ch3,Ch3Result,Ch3Target,Ch3Range,Ch3Units,Ch3Flag,HgbMult,Expiration,DiagFlags";
            }
        }
        public string ToCSV
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(QCSampleTime).Append(",")
                    .Append(OID.TrimEnd('\0')).Append(",")
                    .Append(LotCode.TrimEnd('\0')).Append(",")
                    .Append(SampleType).Append(",")
                    .Append(Ch1Name).Append(",")
                    .Append(Ch1Result).Append(",")
                    .Append(Ch1Target).Append(",")
                    .Append(Ch1Range).Append(",")
                    .Append(Ch1Units).Append(",")
                    .Append(Ch1Flags).Append(",")
                    .Append(Ch2Name).Append(",")
                    .Append(Ch2Result).Append(",")
                    .Append(Ch2Target).Append(",")
                    .Append(Ch2Range).Append(",")
                    .Append(Ch2Units).Append(",")
                    .Append(Ch2Flags).Append(",")
                    .Append(Ch3Name).Append(",")
                    .Append(Ch3Result).Append(",")
                    .Append(Ch3Target).Append(",")
                    .Append(Ch3Range).Append(",")
                    .Append(Ch3Units).Append(",")
                    .Append(Ch3Flags).Append(",")
                    .Append(HgbMultAppliedFlag).Append(",")
                    .Append(Expiration).Append(",")
                    .Append(DiagFlags).Append(",");

                return builder.ToString();
            }
        }

        public bool Sent
        {
            get
            {
                return SerialMessage.IsBitSet(sample_status, Constants.SENT_BIT);
            }
        }

        public string Ch1Name
        {
            get
            {
                if (SerialMessage.IsSensorID(sample_status, Constants.LEAD_SENSOR_ID))
                    return ("Pb");

                if (SerialMessage.IsSensorID(sample_status, Constants.LEAD_HGB_SENSOR_ID))
                    return ("Pb");

                if (SerialMessage.IsSensorID(sample_status, Constants.BILIRUBIN_SENSOR_ID))
                    return ("tBili");

                return ("Unknown");
            }
        }

        public string Ch2Name
        {
            get
            {
                if (SerialMessage.IsSensorID(sample_status, Constants.LEAD_SENSOR_ID))
                    return ("");

                if (SerialMessage.IsSensorID(sample_status, Constants.LEAD_HGB_SENSOR_ID))
                    return ("Hgb");

                if (SerialMessage.IsSensorID(sample_status, Constants.BILIRUBIN_SENSOR_ID))
                    return ("");

                return ("Unknown");
            }
        }

        public string Ch3Name
        {
            get
            {
                if (SerialMessage.IsSensorID(sample_status, Constants.LEAD_SENSOR_ID))
                    return ("");

                if (SerialMessage.IsSensorID(sample_status, Constants.LEAD_HGB_SENSOR_ID))
                    return ("");

                if (SerialMessage.IsSensorID(sample_status, Constants.BILIRUBIN_SENSOR_ID))
                    return ("");

                return ("Unknown");
            }
        }

        public string Ch1Result
        {
            get
            {
                return Result(Results[0].result);
            }
        }

        public string Ch2Result
        {
            get
            {
                return Result(Results[1].result);
            }
        }

        public string Ch3Result
        {
            get
            {
                return Result(Results[2].result);
            }
        }

        public string Target(ushort target)
        {
            if( target > 0 )
            {
                return string.Format("{0:N1}", (float)target/10);
            }
            return "0";
        }

        public string Ch1Target
        {
            get
            {
                return Target(Results[0].target);
            }
        }

        public float Target(short channel)
        {
            return (float)Results[channel].target/10;
        }

        public string Ch2Target
        {
            get
            {
                return Target(Results[1].target);
            }
        }

        public string Ch3Target
        {
            get
            {
                return Target(Results[2].target);
            }
        }

        public string Range(ushort range)
        {
            if( range > 0 )
            {
                return string.Format("{0:N1}", (float)range/10);
            }
            return "0";
        }

        public float Range(short channel)
        {
            
            return (float)Results[channel].range/10;
        }

        /// <summary>
        /// Pb range value formatted string % 10
        /// </summary>
        public string Ch1Range
        {
            get
            {
                return Range(Results[0].range);
            }
        }

        public string Ch2Range
        {
            get
            {
                return Range(Results[1].range);
            }
        }

        public string Ch3Range
        {
            get
            {
                return Range(Results[2].range);
            }
        }

        public string Ch1Units
        {
            get
            {
                return Units(Results[0].mask);
            }
        }

        public string Ch2Units
        {
            get
            {
                return Units(Results[1].mask);
            }
        }

        public string Ch3Units
        {
            get
            {
                return Units(Results[2].mask);
            }
        }

        public string Ch1Flags
        {
            get
            {
                return Flag(Results[0].mask);
            }
        }

        public string Ch2Flags
        {
            get
            {
                return Flag(Results[1].mask);
            }
        }

        public string Ch3Flags
        {
            get
            {
                return Flag(Results[2].mask);
            }
        }

        public string Expiration
        {
            get
            {
                return UnixTime.FromUnixTime(expiration).ToShortDateString();
            }
        }

        public string HgbMultAppliedFlag
        {
            get
            {
                if (HgbMultApplied > 0)
                    return "1";
                else
                    return "0";
            }
        }

        public ushort Result(short channel)
        {
            return Results[channel].result;
        }

        public UnitsEnum Units(short channel)
        {
            return ToUnits(Results[channel].mask);
        }

        public FlagsEnum Flag(short channel)
        {
            return ToFlags(Results[channel].mask);
        }
        public int Level
        {
            get
            {
                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_1_BITS))
                {
                    return 1;
                }
                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_2_BITS))
                {
                    return 2;
                }
                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_3_BITS))
                {
                    return 3;
                }
                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_4_BITS))
                {
                    return 4;
                }
                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_5_BITS))
                {
                    return 5;
                }

                // we need to default to something
                return 6;
            }
        }

        public QCResult(byte[] data)
        {
            int idx = 0;
            sample_status = (UInt32)SerialMessage.PackDWord(data, idx);
            idx += 4;

            // Pb
            Results[0] = new QCMeasuredResults();
            Results[0].mask = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[0].result = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[0].target = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[0].range = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;

            // Hgb
            Results[1] = new QCMeasuredResults();
            Results[1].mask = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[1].result = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[1].target = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[1].range = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;

            // Hct
            Results[2] = new QCMeasuredResults();
            Results[2].mask = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[2].result = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[2].target = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[2].range = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;

            LotCode = Encoding.ASCII.GetString(data, idx, 6);
            idx += 6;

            OID = Encoding.ASCII.GetString(data, idx, 15);
            idx += 15;

            HgbMultApplied = data[idx];
            idx += 1;

            DiagFlags = SerialMessage.PackWord(data, idx);
            idx += 2;

            timestamp = (int)SerialMessage.PackDWord(data, idx);
            idx += 4;

            timeOffsetMins = (Int16)SerialMessage.PackWord(data, idx);
            idx += 2;

            expiration = (int)SerialMessage.PackDWord(data, idx);
            idx += 4;

            crc = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
        }
    }

    public class QCMeasuredResults
    {
        public UInt16 mask;
        public UInt16 result;
        public UInt16 target;
        public UInt16 range;

        public QCMeasuredResults()
        {
            mask = 0;
            result = 0;
            target = 0;
            range = 0;
        }
    }

}
