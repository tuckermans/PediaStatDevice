using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class POSTLog
    {
        public int time;                       // power on time (MUST be first field)
        public short timeOffsetMins;
        public short temp;
        public short UIConfig;
        public short RTConfig;
        public short dacaPosRail;
        public short dacaZero;
        public short dacaNegRail;
        public short meas1SingleElectrodePos;
        public short meas1SingleElectrodeZero;
        public short meas1SingleElectrodeNeg;
        public short meas1DummyElectrodeGain3Pos;
        public short meas1DummyElectrodeGain2Pos;
        public short meas1DummyElectrodeGain1Pos;
        public short meas1DummyElectrodeGain0Pos;
        public short meas1DummyElectrodeGain3Neg;
        public short meas1DummyElectrodeGain2Neg;
        public short meas1DummyElectrodeGain1Neg;
        public short meas1DummyElectrodeGain0Neg;

        public short opticalOffIntensity;
        public short opticalCurrentViolet;
        public short opticalIntensityViolet;
        public short opticalCurrentBlue;
        public short opticalIntensityBlue;
        public short opticalCurrentGreen;
        public short opticalIntensityGreen;
        public short opticalCurrentRed;
        public short opticalIntensityRed;
        public short opticalCurrentLed4;
        public short opticalIntensityLed4;
        public short opticalCurrentLed5;
        public short opticalIntensityLed5;
        public short opticalCurrentLed6;
        public short opticalIntensityLed6;
        public short opticalCurrentLed7;
        public short opticalIntensityLed7;
        public short fanSpeed;

        public UInt16 crc;                        // record CRC (MUST be last field in struct)

        public string ToCSV
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(POSTTime).Append(",")
                    .Append(temp).Append(",")
                    .Append(UIConfig).Append(",")
                    .Append(RTConfig).Append(",")
                    .Append(dacaPosRail).Append(",")
                    .Append(dacaZero).Append(",")
                    .Append(dacaNegRail).Append(",")
                    .Append(meas1SingleElectrodePos).Append(",")
                    .Append(meas1SingleElectrodeZero).Append(",")
                    .Append(meas1SingleElectrodeNeg).Append(",")
                    .Append(meas1DummyElectrodeGain3Pos).Append(",")
                    .Append(meas1DummyElectrodeGain2Pos).Append(",")
                    .Append(meas1DummyElectrodeGain1Pos).Append(",")
                    .Append(meas1DummyElectrodeGain0Pos).Append(",")
                    .Append(meas1DummyElectrodeGain3Neg).Append(",")
                    .Append(meas1DummyElectrodeGain2Neg).Append(",")
                    .Append(meas1DummyElectrodeGain1Neg).Append(",")
                    .Append(meas1DummyElectrodeGain0Neg).Append(",")
                    .Append(opticalOffIntensity).Append(",")
                    .Append(opticalCurrentViolet).Append(",")
                    .Append(opticalIntensityViolet).Append(",")
                    .Append(opticalCurrentBlue).Append(",")
                    .Append(opticalIntensityBlue).Append(",")
                    .Append(opticalCurrentGreen).Append(",")
                    .Append(opticalIntensityGreen).Append(",")
                    .Append(opticalCurrentRed).Append(",")
                    .Append(opticalIntensityRed).Append(",")
                    .Append(opticalCurrentLed4).Append(",")
                    .Append(opticalIntensityLed4).Append(",")
                    .Append(opticalCurrentLed5).Append(",")
                    .Append(opticalIntensityLed5).Append(",")
                    .Append(opticalCurrentLed6).Append(",")
                    .Append(opticalIntensityLed6).Append(",")
                    .Append(opticalCurrentLed7).Append(",")
                    .Append(opticalIntensityLed7).Append(",")
                    .Append(fanSpeed).Append(",");

                return builder.ToString();
            }

        }

        public string POSTTime
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

        public POSTLog(byte[] data)
        {
            int idx = 0;
            time = (Int32) SerialMessage.PackDWord(data, idx);
            idx += 4;

            timeOffsetMins = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            UIConfig = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            RTConfig = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            temp = (Int16)SerialMessage.PackWord(data, idx);
            idx += 2;

            dacaPosRail = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            
            dacaZero    = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            
            dacaNegRail = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1SingleElectrodePos = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1SingleElectrodeZero = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1SingleElectrodeNeg = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1DummyElectrodeGain3Pos = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1DummyElectrodeGain2Pos = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1DummyElectrodeGain1Pos = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1DummyElectrodeGain0Pos = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1DummyElectrodeGain3Neg = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1DummyElectrodeGain2Neg = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1DummyElectrodeGain1Neg = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            meas1DummyElectrodeGain0Neg = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            opticalOffIntensity = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalCurrentViolet = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalIntensityViolet = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalCurrentBlue = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalIntensityBlue = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalCurrentGreen = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalIntensityGreen = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalCurrentRed = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalIntensityRed = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            opticalCurrentLed4 = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalIntensityLed4 = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalCurrentLed5 = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalIntensityLed5 = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalCurrentLed6 = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalIntensityLed6 = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalCurrentLed7 = (short)SerialMessage.PackWord(data, idx);
            idx += 2;
            opticalIntensityLed7 = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            fanSpeed = (short)SerialMessage.PackWord(data, idx);
            idx += 2;

            crc = (UInt16)SerialMessage.PackWord(data, idx);
        }
    }
}
