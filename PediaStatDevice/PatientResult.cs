using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
   
    public class BaseResult
    {
        public UInt16 crc;
        protected ulong sample_status;
        public string LotCode;     // Lot #
        protected int timestamp;
        public Int16 timeOffsetMins;              // Local Time Offset  

        public SampleSourceEnum SampleSource
        {
            get
            {
                if (SerialMessage.IsBitSet(sample_status, Constants.CAPILLARY_BIT) == true)
                {
                    return SampleSourceEnum.Capillary;
                }

                if (SerialMessage.IsBitSet(sample_status, Constants.VENOUS_BIT))
                {
                    return SampleSourceEnum.Venous;
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_1_BITS))
                {
                    return SampleSourceEnum.QCLevel1;
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_2_BITS))
                {
                    return SampleSourceEnum.QCLevel2;
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_3_BITS))
                {
                    return SampleSourceEnum.QCLevel3;
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_4_BITS))
                {
                    return SampleSourceEnum.HGBLevel1;
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_5_BITS))
                {
                    return SampleSourceEnum.HGBlevel2;
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_6_BITS))
                {
                    return SampleSourceEnum.HGBLevel3;
                }
                return SampleSourceEnum.Unknown;

            }
        }
        public string SampleType
        {
            get
            {
                if (SerialMessage.IsBitSet(sample_status, Constants.CAPILLARY_BIT) == true)
                {
                    return "Capillary";
                }

                if (SerialMessage.IsBitSet(sample_status, Constants.VENOUS_BIT))
                {
                    return "Venous";
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_1_BITS))
                {
                    return "Level 1";
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_2_BITS))
                {
                    return "Level 2";
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_3_BITS))
                {
                    return "Level 3";
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_4_BITS))
                {
                    return "Hgb Level 1";
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_5_BITS))
                {
                    return "Hgb Level 2";
                }

                if (SerialMessage.IsLevel(sample_status, Constants.LEVEL_6_BITS))
                {
                    return "Hgb Level 3";
                }

                return "Not Defined";

            }
        }

        protected string Units(ushort mask)
        {
            if (SerialMessage.IsBitSet(mask, Constants.G_DL_BIT))
            {
                return "g/dL";
            }

            if (SerialMessage.IsBitSet(mask, Constants.MMOL_L_BIT))
            {
                return "mmol/L";
            }

            if (SerialMessage.IsBitSet(mask, Constants.UG_DL_BIT))
            {
                return "ug/dL";
            }

            if (SerialMessage.IsBitSet(mask, Constants.PER_EST_BIT))
            {
                return "% est.";
            }

            if (SerialMessage.IsBitSet(mask, Constants.MG_DL_BIT))
            {
                return "mg/dL";
            }

            if (SerialMessage.IsBitSet(mask, Constants.NG_ML_BIT))
            {
                return "ng/mL";
            }

            if (SerialMessage.IsBitSet(mask, Constants.UMOL_L_BIT))
            {
                return "umol/L";
            }

            if (SerialMessage.IsBitSet(mask, Constants.PMOL_L_BIT))
            {
                return "pmol/L";
            }

            return "None";
        }

        protected FlagsEnum ToFlags(ushort mask)
        {
            if (SerialMessage.IsBitSet(mask, Constants.RESULT_HIGH_BIT))
            {
                return FlagsEnum.High;
            }
            else if (SerialMessage.IsBitSet(mask, Constants.RESULT_LOW_BIT))
            {
                return FlagsEnum.Low;
            }
            else if (SerialMessage.IsBitSet(mask, Constants.RESULT_INCALC_BIT))
            {
                return FlagsEnum.Incalc;
            }
            else
            {
                return FlagsEnum.None;
            }
           
        }
        protected UnitsEnum ToUnits(ushort mask)
        {
            if (SerialMessage.IsBitSet(mask, Constants.G_DL_BIT))
            {
                return UnitsEnum.g_dL;
            }

            if (SerialMessage.IsBitSet(mask, Constants.MMOL_L_BIT))
            {
                return UnitsEnum.mm_L;
            }

            if (SerialMessage.IsBitSet(mask, Constants.UG_DL_BIT))
            {
                return UnitsEnum.ug_dL;
            }

            if (SerialMessage.IsBitSet(mask, Constants.PER_EST_BIT))
            {
                return UnitsEnum.per_est;
            }

            if (SerialMessage.IsBitSet(mask, Constants.MG_DL_BIT))
            {
                return UnitsEnum.mg_dL;
            }

            if (SerialMessage.IsBitSet(mask, Constants.NG_ML_BIT))
            {
                return UnitsEnum.ng_mL;
            }

            if (SerialMessage.IsBitSet(mask, Constants.UMOL_L_BIT))
            {
                return UnitsEnum.umol_L;
            }

            if (SerialMessage.IsBitSet(mask, Constants.PMOL_L_BIT))
            {
                return UnitsEnum.pmol_L;
            }

            return UnitsEnum.None;
        }

        protected string Result(ushort result)
        {
            if (result > 0)
            {
                return string.Format("{0:0000.0}", (float)result / 10);
            }
            return "0";

        }
      
        protected string Flag(ushort mask)
        {

            if (SerialMessage.IsBitSet(mask, Constants.RESULT_HIGH_BIT))
            {
                return "High";
            }
            if (SerialMessage.IsBitSet(mask, Constants.RESULT_LOW_BIT))
            {
                return "Low";
            }
            if (SerialMessage.IsBitSet(mask, Constants.RESULT_INCALC_BIT))
            {
                return "InCalc";
            }

            return "None";
        }

        public string SampleTime
        {
            get
            {
                string temp;
                temp = UnixTime.FromUnixTime(timestamp).ToString("yyyy-MM-ddTHH:mm:ss");

                int offsetMins, offsetSecs;
                offsetMins = Math.Abs(timeOffsetMins / 60);
                offsetSecs = Math.Abs(timeOffsetMins % 60);

                if( timeOffsetMins >= 0)
                    return string.Format("{0}+{1:D2}:{2:D2}", temp, offsetMins, offsetSecs);
                else
                    return string.Format("{0}-{1:D2}:{2:D2}", temp, offsetMins, offsetSecs);
            }
        }

        public SensorIDEnum SensorID
        {
            get
            {
                if (SerialMessage.IsSensorID(sample_status, Constants.LEAD_SENSOR_ID))
                {
                    return SensorIDEnum.LEAD_SENSOR_ID;
                }
                else if (SerialMessage.IsSensorID(sample_status, Constants.LEAD_HGB_SENSOR_ID))
                {
                    return SensorIDEnum.LEAD_HGB_SENSOR_ID;
                }
                else if (SerialMessage.IsSensorID(sample_status, Constants.CHOLESTEROL_SENSOR_ID))
                {
                    return SensorIDEnum.CHOLESTEROL_SENSOR_ID;
                }
                else if (SerialMessage.IsSensorID(sample_status, Constants.BILIRUBIN_SENSOR_ID))
                {
                    return SensorIDEnum.BILLIRUBIN_SENSOR_ID;
                }
                else
                {
                    return SensorIDEnum.ELECTRONIC_SENSOR_ID;
                }

            }
        }
    }
    public class PatientResult : BaseResult 
    {
        public MeasuredResults[] Results = new MeasuredResults[3];
        public string PID;    // Patient ID
        public string OID;     // Operator ID (future)
        public byte HgbMultApplied;
        public short DiagFlags;

        public static string Header
        {
            get
            {
                return "Time,PID,OID,LotCode,SampleType,Ch1,Ch1Result,Ch1Units,Ch1Flag,Ch2,Ch2Result,Ch2Units,Ch2Flag,Ch3,Ch3Result,Ch3Units,Ch3Flag,HgbMult,DiagFlags";                
            }
        }

        public string ToCSV
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(SampleTime).Append(",")
                    .Append(PID.TrimEnd('\0')).Append(",")
                    .Append(OID.TrimEnd('\0')).Append(",")
                    .Append(LotCode.TrimEnd('\0')).Append(",")
                    .Append(SampleType).Append(",")
                    .Append(Ch1Name).Append(",")
                    .Append(Ch1Result).Append(",")
                    .Append(Ch1Units).Append(",")
                    .Append(Ch1Flags).Append(",")
                    .Append(Ch2Name).Append(",")
                    .Append(Ch2Result).Append(",")
                    .Append(Ch2Units).Append(",")
                    .Append(Ch2Flags).Append(",")
                    .Append(Ch3Name).Append(",")
                    .Append(Ch3Result).Append(",")
                    .Append(Ch3Units).Append(",")
                    .Append(Ch3Flags).Append(",")
                    .Append(HgbMultAppliedFlag).Append(",")
                    .Append(DiagFlags).Append(",");

                return builder.ToString();
            }
        }

        public bool Sent
        {
            get
            {
                return (true == SerialMessage.IsBitSet(sample_status, Constants.SENT_BIT));
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

        public ushort Result(short channel)
        {
            return Results[channel].result;
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
                    return ("Bkm");

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
                    return ("Hct");

                if (SerialMessage.IsSensorID(sample_status, Constants.BILIRUBIN_SENSOR_ID))
                    return ("Gkm");

                return ("Unknown");
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

        public UnitsEnum Units(short channel)
        {
            return ToUnits(Results[channel].mask);
        }

        public FlagsEnum Flag(short channel)
        {
            return ToFlags(Results[channel].mask);
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

        public int Timestamp
        {
            get
            {
                return timestamp;
            }
        }

 

        public PatientResult(byte[] data)
        {
            int idx = 0;

            sample_status = (UInt32)SerialMessage.PackDWord(data, idx);
            idx += 4;
            
            // Pb
            Results[0] = new MeasuredResults();
            Results[0].mask = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[0].result = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;

            // Hgb
            Results[1] = new MeasuredResults();
            Results[1].mask = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[1].result = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            
            // Hct 
            Results[2] = new MeasuredResults();
            Results[2].mask = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
            Results[2].result = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;

            LotCode = Encoding.ASCII.GetString(data, idx, 6);
            idx += 6;
            PID = Encoding.ASCII.GetString(data, idx, 20);
            idx += 20;
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

            crc = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;
        }
    }

    public class MeasuredResults
    {
        public UInt16 mask;
        public UInt16 result;

        public MeasuredResults()
        {
           mask = 0;
           result = 0;
        }
    }
}
