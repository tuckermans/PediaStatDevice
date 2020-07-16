using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public enum ChannelEnum : short
    {
        Channel1 = 1,
        Channel2,
        Channel3
    }

    public class QCRanges : SerialMessage
    {
        public QCRanges()
        {
            Target = 0;
            Range = 0;

        }
        public QCRanges(short target, short range)
        {
            Target = target;
            Range   = range;
        }

        public float Target
        { get; set; }

        public float Range
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Class represents the interface to a button to the outside world
    /// this is intended to insulate the users of this object from the 
    /// details of how the object is constructed and initialized
    /// </summary>
    public class QCButtonData
    {
        public List<QCRanges> QCLevel1 = new List<QCRanges>();
        public List<QCRanges> QCLevel2 = new List<QCRanges>();
        public List<QCRanges> QCLevel3 = new List<QCRanges>();


        public QCButtonData()
        {
           
        }

        public string ButtonTime
        {
            get
            {
                string temp;
                temp = UnixTime.FromUnixTime((int)timestamp).ToString("yyyy-MM-ddTHH:mm:ss");

                int offsetMins, offsetSecs;
                offsetMins = Math.Abs(timeOffsetMins / 60);
                offsetSecs = Math.Abs(timeOffsetMins % 60);

                if (timeOffsetMins >= 0)
                    return string.Format("{0}+{1:D2}:{2:D2}", temp, offsetMins, offsetSecs);
                else
                    return string.Format("{0}-{1:D2}:{2:D2}", temp, offsetMins, offsetSecs);
            }
        }

        public static string Header
        {
            get
            {
                return "Time,LotCode,LotCRC,Expiration,Ch1,Ch1LowLimit,Ch1UpperLimit,Ch1Level1.Target,Ch1Level1.Range,Ch1Level2.Target,Ch1Level2.Range,Ch1Level3.Target,Ch1Level3.Range,Ch2,Ch2LowLimit,Ch2HighLimit,Ch2Level1.Target,Ch2Level1.Range,Ch2Level2.Target,Ch2Level2.Range,Ch2Level3.Target,Ch2Level3.Range,Ch3,Ch3LowLimit,Ch3HighLimit,Ch3Level1.Target,Ch3Level1.Range,Ch3Level2.Target,Ch3Level2.Range,Ch3Level3.Target,Ch3Level3.Range";
            }
        }
        public string ToCSV
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(ButtonTime).Append(",")
                    .Append(LotCode.TrimEnd('\0')).Append(",")
                    .Append(LotCRC).Append(",")
                    .Append(Expiration.ToShortDateString().TrimEnd('\0')).Append(",")
                    .Append(Chan1Name).Append(",")
                    .Append(Chan1Ranges.LowerLimit).Append(",")
                    .Append(Chan1Ranges.UpperLimit).Append(",")
                    .Append((float)(Chan1Level1.Target)).Append(",")
                    .Append((float)(Chan1Level1.Range)).Append(",")
                    .Append((float)(Chan1Level2.Target)).Append(",")
                    .Append((float)(Chan1Level2.Range)).Append(",")
                    .Append((float)(Chan1Level3.Target)).Append(",")
                    .Append((float)(Chan1Level3.Range)).Append(",")
                    .Append(Chan2Name).Append(",")
                    .Append(Chan2Ranges.LowerLimit).Append(",")
                    .Append(Chan2Ranges.UpperLimit).Append(",")
                    .Append((float)(Chan2Level1.Target)).Append(",")
                    .Append((float)(Chan2Level1.Range)).Append(",")
                    .Append((float)(Chan2Level2.Target)).Append(",")
                    .Append((float)(Chan2Level2.Range)).Append(",")
                    .Append((float)(Chan2Level3.Target)).Append(",")
                    .Append((float)(Chan2Level3.Range)).Append(",")
                    .Append(Chan3Name).Append(",")
                    .Append(Chan3Ranges.LowerLimit).Append(",")
                    .Append(Chan3Ranges.UpperLimit).Append(",")
                    .Append((float)(Chan3Level1.Target)).Append(",")
                    .Append((float)(Chan3Level1.Range)).Append(",")
                    .Append((float)(Chan3Level2.Target)).Append(",")
                    .Append((float)(Chan3Level2.Range)).Append(",")
                    .Append((float)(Chan3Level3.Target)).Append(",")
                    .Append((float)(Chan3Level3.Range)).Append(",");
                return builder.ToString();
            }
        }
        
        public String LotCode
        {
            get;
            set;
        }

        public DateTime Expiration
        {
            get;
            set;
        }
    
        public short MeasureTime
        {
            get;
            set;
        }

        public QCRanges Chan1Level1
        {
            get;
            set;
        }
        public QCRanges Chan1Level2
        {
            get;
            set;
        }
        public QCRanges Chan1Level3
        {
            get;
            set;
        }

        public QCRanges Chan2Level1
        {
            get;
            set;
        }
        public QCRanges Chan2Level2
        {
            get;
            set;
        }
        public QCRanges Chan2Level3
        {
            get;
            set;
        }

        public QCRanges Chan3Level1
        {
            get;
            set;
        }
        public QCRanges Chan3Level2
        {
            get;
            set;
        }
        public QCRanges Chan3Level3
        {
            get;
            set;
        }

        public SensorIDEnum SensorID
        {
            get;
            set;
        }

        public string Chan1Name
        {
            get
            {
                if (SensorID == SensorIDEnum.LEAD_SENSOR_ID || SensorID == SensorIDEnum.LEAD_HGB_SENSOR_ID)
                {
                    return "Pb";
                }
                else if (SensorID == SensorIDEnum.BILLIRUBIN_SENSOR_ID)
                {
                    return "tBili";
                }
                else if (SensorID == SensorIDEnum.CHOLESTEROL_SENSOR_ID)
                {
                    return "Hdl";
                }
                return "--";
            }
        }
        public string Chan2Name
        {
            get
            {
                if (SensorID == SensorIDEnum.LEAD_HGB_SENSOR_ID)
                {
                    return "Hgb";
                }
                return "--";
            }
        }

        public string Chan3Name
        {
            get
            {
                return "--";
            }
        }

        /// <summary>
        /// Gets the QCRanges by channel and level
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public QCRanges getQCLevel(ChannelEnum channel, short level)
        {
            short index = (short)(channel - 1);

            switch(level)
            {
            
                case 1:
                    if (QCLevel1.Count > (short)channel)
                    {
                        
                        return QCLevel1[index];
                    }
                    break;
                case 2:
                    {
                        if (QCLevel2.Count > (short)channel)
                        {
                            return QCLevel2[index];
                            
                        }
                        break;
                    }
                default:
                    if (QCLevel3.Count > (short)channel)
                    {
                        return QCLevel3[index];
                    }
                    break;
            }
            return null;
        }

     
        
     
        /// <summary>
        /// Returns the measurable ranges by channel
        /// </summary>
        /// <param name="channel">ChannelEnum - Channel Number</param>
        /// <returns></returns>
        public Range getMeasureRange(ChannelEnum channel)
        {
            switch (channel)
            {
                case ChannelEnum.Channel1:
                    return Chan1Ranges;
                case ChannelEnum.Channel2:
                    return Chan2Ranges;
                case ChannelEnum.Channel3:
                    return Chan3Ranges;
            }
            return null;
        }

        /// <summary>
        /// Channel1 Measurable Ranges
        /// </summary>
        public Range Chan1Ranges
        {
            get;
            set;
        }

        public Range Chan2Ranges
        {
            get;
            set;
        }

        public Range Chan3Ranges
        {
            get;
            set;
        }

        public UInt16 LotCRC
        {
            get;
            set;
        }

        public uint timestamp
        {
            get;
            set;
        }

        public Int16 timeOffsetMins
        {
            get;
            set;
        }
    }
}