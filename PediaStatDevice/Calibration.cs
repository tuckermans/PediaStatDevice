using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PediaStatDevice
{

    //using CRCmodule = ESA.LeadCare.CRC;		// avoid conflict with calibration CRC

    [Serializable]
    public struct swcEntry
    {
        public ushort swc;
        public ushort bll;
    }

    [Serializable]
    public struct tempEntry
    {
        public ushort temp;
        public ushort factor;
    }

    /// <summary>
    /// LeadCare II calibration
    /// </summary>
    [Serializable]
    public class Calibration : ICloneable
    {
        Crc16 _crc = new Crc16();

        public const int NumBinaryBytes = 442;
        public const int NumLeadEntries = 64;
        public const int NumTempEntries = 32;

        [Flags]
        public enum DevelopSwitch : short
        {
            DisplayTemp = 1, IgnoreRemoval = 2, ShowTechQC = 4, DisableShutoff = 8
        }

        public uint MaxADCcount = 262144;
        public short TempMin = 120;
        public short TempMax = 400;
        public short TempChangeMax = 5;
        public short DelayTime = 20;
        public short DelaymVolts = 0;
        public short P1Time = 10;
        public short P1mVolts = -500;
        public short P2Time = 10;
        public short P2mVolts = -50;
        public short DepTime = 140;
        public short DepmVolts = -530;
        public short FinalmVolts = 100;
        public short SWFreq = 115;
        public short OffsetmVolts = 25;
        public short StepSize = 3;
        public short Gain = 2;
        public short RampRate = 25;
        public short Min1 = 25;
        public short Min2 = 110;
        public short Min3 = 135;
        public short Min4 = 190;
        public short SensorTimeLimit = 0;
        public short Switches = 0;
        public short Date = 2975;
        public short LotCodeNum = 1;
        public short LotCodeChar = 88;
        public swcEntry[] SwcTable = new swcEntry[NumLeadEntries];
        public tempEntry[] TempTable = new tempEntry[NumTempEntries];
        public ushort CRC;
        public string Note = "";

        //
        public int Level1_Min = 0;
        public int Level1_Max = 0;
        public int Level2_Min = 0;
        public int Level2_Max = 0;
        public int Level3_Min = 0;
        public int Level3_Max = 0;

        //
        private const int ShortTempTableMarkIndex = 21;
        private const int MinGoodSwcIndex = NumTempEntries - 1;

        private ushort CRCreceived;

        /// <summary>
        /// Constructor
        /// </summary>
        public Calibration()
        {
        }

        /// <summary>
        /// deep clone
        /// </summary>
        /// <returns>Calibration object</returns>
        public object Clone()
        {
            BinaryFormatter BF = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();

            BF.Serialize(memStream, this);
            memStream.Flush();
            memStream.Position = 0;

            return (BF.Deserialize(memStream));
        }

        /// <summary>
        /// read only property forms lotcode from lot number and character
        /// </summary>
        public string LotCode
        {
            get
            {
                return LotCodeNum.ToString("D4") + Convert.ToChar(LotCodeChar);
            }
            set
            {
            }
        }

        /// <summary>
        /// convert date code in button to/from PC DateTime format
        /// Date is (bitwise) yyyy yyym mmmd dddd 
        /// </summary>
        public DateTime ExpirationDate
        {
            get
            {
                int day = Date % 32;
                int month = (Date / 32) % 16;
                int year = (Date / 512) + 2000;

                DateTime exp = new DateTime(year, month, day, 0, 0, 0, 0);
                return exp;
            }
            set
            {
                Date = (short)(value.Day + (32 * value.Month) + (512 * (value.Year - 2000)));
            }
        }


        private bool SwitchOn(DevelopSwitch switchToTest)
        {
            return (((DevelopSwitch)Switches) & switchToTest) == switchToTest;
        }


        private void SetSwitch(DevelopSwitch switchToChange, bool newValue)
        {
            DevelopSwitch allSwitches = (DevelopSwitch)Switches;
            if (newValue)
            {
                allSwitches |= switchToChange;
            }
            else
                allSwitches &= ~switchToChange;

            Switches = (short)allSwitches;
        }

        /*****
         * QC Range values stored in the button
         *****/
        [XmlIgnore]
        public int Level1MinRange
        {
            get
            {
                return TempTable[NumTempEntries - 5].temp;
            }
        }

        [XmlIgnore]
        public int Level1MaxRange
        {
            get
            {
                return TempTable[NumTempEntries - 5].factor;
            }
        }

        [XmlIgnore]
        public int Level2MinRange
        {
            get
            {
                return TempTable[NumTempEntries - 4].temp;
            }
        }

        [XmlIgnore]
        public int Level2MaxRange
        {
            get
            {
                return TempTable[NumTempEntries - 4].factor;
            }
        }
        [XmlIgnore]
        public int Level3MinRange
        {
            get
            {
                return TempTable[NumTempEntries - 3].temp;
            }
        }

        [XmlIgnore]
        public int Level3MaxRange
        {
            get
            {
                return TempTable[NumTempEntries - 3].factor;
            }
        }

        [XmlIgnore]
        public bool DisplayTemp
        {
            get { return SwitchOn(DevelopSwitch.DisplayTemp); }
            set { SetSwitch(DevelopSwitch.DisplayTemp, value); }
        }

        [XmlIgnore]
        public bool IgnoreRemoval
        {
            get { return SwitchOn(DevelopSwitch.IgnoreRemoval); }
            set { SetSwitch(DevelopSwitch.IgnoreRemoval, value); }
        }

        [XmlIgnore]
        public bool ShowTechQC
        {
            get { return SwitchOn(DevelopSwitch.ShowTechQC); }
            set { SetSwitch(DevelopSwitch.ShowTechQC, value); }
        }

        [XmlIgnore]
        public bool DisableShutoff
        {
            get { return SwitchOn(DevelopSwitch.DisableShutoff); }
            set { SetSwitch(DevelopSwitch.DisableShutoff, value); }
        }

        [XmlIgnore]
        public bool NoGoldTestEnabled
        {
            get
            {
                return ((TempTable[ShortTempTableMarkIndex].temp == 0)
                         && (TempTable[MinGoodSwcIndex].temp == 1));
            }
            set
            {
                if (value)
                {
                    TempTable[ShortTempTableMarkIndex].temp = 0;
                    TempTable[MinGoodSwcIndex].temp = 1;
                }
                else
                {
                    if (TempTable[ShortTempTableMarkIndex].temp == 0)
                    {
                        TempTable[MinGoodSwcIndex].temp = 0;
                    }
                }
            }
        }

        /// <summary>
        /// minimum allowable scaled SWC value for a good sensor
        /// </summary>
        [XmlIgnore]
        public short MinimumGoodSwc
        {
            get
            {
                short minimumSwc = 0;
                // the offset is defined in button only if there is a short temp table
                // and the feature present flag is set
                if (NoGoldTestEnabled)
                {
                    minimumSwc = (short)TempTable[MinGoodSwcIndex].factor;
                }
                return minimumSwc;
            }
            set
            {
                TempTable[MinGoodSwcIndex].factor = (ushort)value;
            }
        }

        public static Calibration Read(string fileName)
        {
            XmlSerializer sr = new XmlSerializer(typeof(Calibration));
            FileStream fs = null;

            try
            {
                fs = File.OpenRead(fileName);
                Calibration cal = (Calibration)sr.Deserialize(fs);
                cal.ReCalculateCRC(); // spreadsheet does not produce CRC, so always recalc
                return cal;
            }
            catch (Exception)
            {
                throw new ApplicationException("Calibration file read error " + fileName);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public virtual void Write(string fileName)
        {
            XmlSerializer sr = new XmlSerializer(typeof(Calibration));
            StreamWriter sw = null;

            try
            {
                this.ReCalculateCRC(); // force recalc to insure integrity
                sw = new StreamWriter(fileName);
                sr.Serialize(sw, this);
            }
            catch (Exception )
            {
            }
            finally
            {
                sw.Close();
            }
        }

        public void ConvertFromBinary(byte[] binaryData)
        {
            int i = 0;		// current integer (2 byte value) positon in binary data stream

            for (int row = 0; row < NumLeadEntries; row++)
            {
                SwcTable[row].swc = BitConverter.ToUInt16(binaryData, i++ * 2);
                SwcTable[row].bll = BitConverter.ToUInt16(binaryData, i++ * 2);
            }

            for (int row = 0; row < NumTempEntries; row++)
            {
                TempTable[row].temp = BitConverter.ToUInt16(binaryData, i++ * 2);
                TempTable[row].factor = BitConverter.ToUInt16(binaryData, i++ * 2);
            }

            MaxADCcount = BitConverter.ToUInt32(binaryData, i * 2);
            i += 2;

            TempMin = BitConverter.ToInt16(binaryData, i++ * 2);
            TempMax = BitConverter.ToInt16(binaryData, i++ * 2);
            TempChangeMax = BitConverter.ToInt16(binaryData, i++ * 2);
            DelayTime = BitConverter.ToInt16(binaryData, i++ * 2);
            DelaymVolts = BitConverter.ToInt16(binaryData, i++ * 2);
            P1Time = BitConverter.ToInt16(binaryData, i++ * 2);
            P1mVolts = BitConverter.ToInt16(binaryData, i++ * 2);
            P2Time = BitConverter.ToInt16(binaryData, i++ * 2);
            P2mVolts = BitConverter.ToInt16(binaryData, i++ * 2);
            DepTime = BitConverter.ToInt16(binaryData, i++ * 2);
            DepmVolts = BitConverter.ToInt16(binaryData, i++ * 2);
            FinalmVolts = BitConverter.ToInt16(binaryData, i++ * 2);
            SWFreq = BitConverter.ToInt16(binaryData, i++ * 2);
            OffsetmVolts = BitConverter.ToInt16(binaryData, i++ * 2);
            StepSize = BitConverter.ToInt16(binaryData, i++ * 2);
            Gain = BitConverter.ToInt16(binaryData, i++ * 2);
            RampRate = BitConverter.ToInt16(binaryData, i++ * 2);
            Min1 = BitConverter.ToInt16(binaryData, i++ * 2);
            Min2 = BitConverter.ToInt16(binaryData, i++ * 2);
            Min3 = BitConverter.ToInt16(binaryData, i++ * 2);
            Min4 = BitConverter.ToInt16(binaryData, i++ * 2);
            SensorTimeLimit = BitConverter.ToInt16(binaryData, i++ * 2);
            Switches = BitConverter.ToInt16(binaryData, i++ * 2);
            Date = BitConverter.ToInt16(binaryData, i++ * 2);
            LotCodeNum = BitConverter.ToInt16(binaryData, i++ * 2);
            LotCodeChar = BitConverter.ToInt16(binaryData, i++ * 2);
            CRCreceived = BitConverter.ToUInt16(binaryData, i++ * 2);
            
            // Extract the QC Levels from the binary data
            Level1_Min = Level1MinRange;
            Level1_Max = Level1MaxRange;

            Level2_Min = Level2MinRange;
            Level2_Max = Level2MaxRange;
            Level3_Min = Level3MinRange;

            Level3_Max = Level3MaxRange;

            ReCalculateCRC();
        }

        private void ConvertShort(byte[] binaryData, int i, short data)
        {
            byte[] temp = BitConverter.GetBytes(data);
            binaryData[2 * i] = temp[0];
            binaryData[2 * i++ + 1] = temp[1];
        }

        public ushort ReCalculateCRC()
        {
            this.ConvertToBinary(); // recalcs CRC as side effect;
            return this.CRC;
        }

        public byte[] ConvertToBinary()
        {
            int i = 0;		// current integer (2 byte value) positon in binary data stream

            byte[] binaryData = new byte[NumBinaryBytes];

            for (int row = 0; row < NumLeadEntries; row++)
            {
                ConvertShort(binaryData, i++, (short)SwcTable[row].swc);
                ConvertShort(binaryData, i++, (short)SwcTable[row].bll);
            }

            for (int row = 0; row < NumTempEntries; row++)
            {
                ConvertShort(binaryData, i++, (short)TempTable[row].temp);
                ConvertShort(binaryData, i++, (short)TempTable[row].factor);
            }

            byte[] temp = BitConverter.GetBytes(MaxADCcount);
            for (int j = 0; j < temp.Length; j++)
                binaryData[i * 2 + j] = temp[j];
            i += 2;

            ConvertShort(binaryData, i++, TempMin);
            ConvertShort(binaryData, i++, TempMax);
            ConvertShort(binaryData, i++, TempChangeMax);
            ConvertShort(binaryData, i++, DelayTime);
            ConvertShort(binaryData, i++, DelaymVolts);
            ConvertShort(binaryData, i++, P1Time);
            ConvertShort(binaryData, i++, P1mVolts);
            ConvertShort(binaryData, i++, P2Time);
            ConvertShort(binaryData, i++, P2mVolts);
            ConvertShort(binaryData, i++, DepTime);
            ConvertShort(binaryData, i++, DepmVolts);
            ConvertShort(binaryData, i++, FinalmVolts);
            ConvertShort(binaryData, i++, SWFreq);
            ConvertShort(binaryData, i++, OffsetmVolts);
            ConvertShort(binaryData, i++, StepSize);
            ConvertShort(binaryData, i++, Gain);
            ConvertShort(binaryData, i++, RampRate);
            ConvertShort(binaryData, i++, Min1);
            ConvertShort(binaryData, i++, Min2);
            ConvertShort(binaryData, i++, Min3);
            ConvertShort(binaryData, i++, Min4);
            ConvertShort(binaryData, i++, SensorTimeLimit);
            ConvertShort(binaryData, i++, Switches);
            ConvertShort(binaryData, i++, Date);
            ConvertShort(binaryData, i++, LotCodeNum);
            ConvertShort(binaryData, i++, LotCodeChar);

            this.CRC = _crc.ComputeChecksum(binaryData, 0, binaryData.Length - 2);

            ConvertShort(binaryData, i++, (short)CRC);

            return binaryData;
        }

        public bool CRCcompare(ushort CRCtoCheck)
        {
            return (CRCtoCheck == this.CRC || CRCtoCheck == this.CRCreceived);
        }

        /// <summary>
        /// compare calibrations 
        /// </summary>
        /// <param name="testCal">calibration to compare</param>
        /// <returns>true if testCal has different value than this</returns>
        public bool Different(Calibration testCal)
        {
            // must differ if test doesn't exist
            if (testCal == null)
                return true;

            // get two binarys and compare, ignore CRC and .Note differences
            byte[] testBinary = testCal.ConvertToBinary();
            byte[] thisBinary = this.ConvertToBinary();
            bool differ = false;

            for (int i = 0; i < thisBinary.Length - 2; i++)
            {
                if (testBinary[i] != thisBinary[i])
                    differ = true;
            }

            return differ;
        }

        private bool RangeCheckTemp(String fieldId, short value, short min, short max, StringBuilder msg)
        {
            bool failed = false;

            if (value < min || value > max)
            {
                failed = true;
                msg.AppendFormat("{0} is outside the expected range:  {1:#0.0} is not within [{2:#0.0} - {3:#0.0}]\n",
                    fieldId, value / 10.0, min / 10.0, max / 10.0);
            }

            return failed;
        }

        private short StepMillivolt(short stepNum)
        {
            return (short)(this.DepmVolts + (this.StepSize * stepNum));
        }

        private bool RangeCheck(String fieldId, short value, short min, short max, StringBuilder msg)
        {
            bool failed = false;

            if (value < min || value > max)
            {
                failed = true;
                msg.AppendFormat("{0} is outside the expected range:  {1:d} is not within [{2:d} - {3:d}]\n",
                    fieldId, value, min, max);
            }

            return failed;
        }

        public bool WellFormed(out String message)
        {
            bool problem = false;
            StringBuilder msg = new StringBuilder();

            problem |= RangeCheckTemp("Minimum Temperature", this.TempMin, 80, 240, msg);
            problem |= RangeCheckTemp("Maximum Temperature", this.TempMax, 260, 480, msg);
            problem |= RangeCheckTemp("Maximum Temperature Change", this.TempChangeMax, 0, 100, msg);

            problem |= RangeCheck("Delay Time", this.DelayTime, 0, 4095, msg);
            problem |= RangeCheck("Delay Potential", this.DelaymVolts, -2000, 2000, msg);
            problem |= RangeCheck("Precondition 1 time", this.P1Time, 0, 4095, msg);
            problem |= RangeCheck("Precondition 1 potential", this.P1mVolts, -2000, 2000, msg);
            problem |= RangeCheck("Precondition 2 time", this.P2Time, 0, 4095, msg);
            problem |= RangeCheck("Precondition 2 potential", this.P2mVolts, -2000, 2000, msg);
            problem |= RangeCheck("Deposition time", this.DepTime, 0, 4095, msg);
            problem |= RangeCheck("Deposition potential", this.DepmVolts, -2000, 2000, msg);
            problem |= RangeCheck("Final potential", this.FinalmVolts, -2000, 2000, msg);

            problem |= RangeCheck("Square Wave Frequency", this.SWFreq, 50, 175, msg);
            problem |= RangeCheck("Forward & Reverse Offsets", this.OffsetmVolts, 1, 99, msg);
            problem |= RangeCheck("Step Size", this.StepSize, 0, 255, msg);
            problem |= RangeCheck("Gain during scan", this.Gain, 1, 3, msg);
            problem |= RangeCheck("Ramp Rate", this.RampRate, 0, 255, msg);

            problem |= RangeCheck("Minimum detection potential #1", StepMillivolt(this.Min1), -2000, 2000, msg);
            problem |= RangeCheck("Minimum detection potential #2", StepMillivolt(this.Min2), -2000, 2000, msg);
            problem |= RangeCheck("Minimum detection potential #3", StepMillivolt(this.Min3), -2000, 2000, msg);
            problem |= RangeCheck("Minimum detection potential #4", StepMillivolt(this.Min4), -2000, 2000, msg);

            // check windows for proper order of edges
            if ((Min1 >= Min2) || (Min2 >= Min3) || (Min3 >= Min4))
            {
                problem |= true;
                msg.AppendFormat("Minima windows badly formed: [({0}) - ({1})] [({2}) - ({3})]\n",
                    StepMillivolt(Min1), StepMillivolt(Min2), StepMillivolt(Min3), StepMillivolt(Min4));
            }

            problem |= RangeCheck("Sensor out of Vial time", this.SensorTimeLimit, 0, 480, msg);

            problem |= RangeCheck("Expiration month", (short)((this.Date / 32) % 16), 1, 12, msg);
            problem |= RangeCheck("Expiration year", (short)(this.Date / 512), 0, 50, msg);

            problem |= RangeCheck("LotCode character", this.LotCodeChar, 65, 90, msg);
            problem |= RangeCheck("LotCode number", this.LotCodeNum, 0, 9999, msg);


            // check SWC (BLL) table - not zero filled at end, too short, y values non-monotonic
            int lastEntry = 0;
            do
            {
                lastEntry++;
            } while (lastEntry < SwcTable.Length - 1 && SwcTable[lastEntry].swc > SwcTable[lastEntry - 1].swc);

            if (SwcTable[lastEntry].swc != 0 && SwcTable[lastEntry].swc <= SwcTable[lastEntry - 1].swc)
            {
                problem |= true;
                msg.AppendFormat("BLL table ends with non-zero SWC: table entry #{0} ({1}) is not greater than previous value.\n",
                    lastEntry + 1, SwcTable[lastEntry].swc);
            }
            else if (lastEntry < SwcTable.Length / 2)
            {
                problem |= true;
                msg.AppendFormat("BLL table is less than half permitted size ({0} entries.)\n", lastEntry + 1);
            }
            // check for BLL monotonicity
            bool monotonic = true;
            for (int i = 0; i < lastEntry; i++)
            {
                if (SwcTable[i].bll >= SwcTable[i + 1].bll)
                {
                    monotonic = false;
                }
            }
            if (!monotonic)
            {
                problem |= true;
                msg.AppendFormat("BLL values are not strictly monotonic (permitted but suspect)\n");
            }

            // check temperature table - not zero filled at end, too short
            lastEntry = 0;
            do
            {
                lastEntry++;
            } while (lastEntry < TempTable.Length - 1 && TempTable[lastEntry].temp > TempTable[lastEntry - 1].temp);

            if (TempTable[lastEntry].temp != 0 && TempTable[lastEntry].temp <= TempTable[lastEntry - 1].temp)
            {
                problem |= true;
                msg.AppendFormat("Temperature table ends with non-zero temperature: table entry #{0} ({1}) is not greater than previous value.\n",
                    lastEntry + 1, TempTable[lastEntry].temp);
            }
            else if (lastEntry < TempTable.Length / 2)
            {
                problem |= true;
                msg.AppendFormat("Temperature table is less than half permitted size ({0} entries.)\n", lastEntry + 1);
            }

            message = msg.ToString();
            if (problem)
            {
            }
            return !problem;
        }
    }
}
