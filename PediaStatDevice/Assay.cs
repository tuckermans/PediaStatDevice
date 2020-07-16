using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace PediaStatDevice
{

    public enum Analyzer
    {
        LeadCare2,
        LeadCare3,
    };

    [Serializable]
    public class rDataRec
    {
        public short[] forward = new short[Assay.NumADCreads];
        public short[] reverse = new short[Assay.NumADCreads];
    }


    /// <summary>
    /// All the test run information and graph customization for one assay
    /// </summary>
    /// 
    [Serializable]
    public class Assay : ICloneable
    {
        public const int NumBinaryBytes = 5170;
        public const int NumADCreads = 16;
        public const uint HighLead = 0xFFFF;
        public const uint LowLead = 0xFFFE;

        private const int MaxStepCount = 512;

        private readonly short[] BoxcarFilter = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private readonly short[] FFTfilter = {256, 278, 297, 314, 328, 340, 349, 356, 360, 361,
							          		  360, 356, 349, 340, 328, 314, 297, 278, 256,     };

        //public GraphOptions graph = new GraphOptions();

        public Calibration cal = new Calibration();

        public string MeterID;

        public byte Valid;
        public byte Error;
        public ushort CalSignature;
        public ushort StepTime;
        public ushort HalfStepTime;
        public short ForwardOffset;
        public short ReverseOffset;
        public ushort StepCount;
        public short TempBeforeDep;
        public short TempAfterDep;
        public short TempAfterAssay;
        public short TempAvg;
        public ushort LowerMinimum;
        public ushort UpperMinimum;
        public short Slope;
        public ushort SWCscaled;

        public int Timestamp;
        public uint SWCValue;
        public uint TempCorrected;
        public uint BLLCorrected;
        public uint Result;

        //[XmlIgnore]
        public int BCiterations = 0;

        public DateTime AssayTimePC;
        public string Note = "";
        public string GroupNote = "";
        public int FileFormat = 2;

        public Analyzer Source = Analyzer.LeadCare2;
        public uint Channel = 0;

        public short[] ForwardTable;
        public short[] ReverseTable;
        public short[] DifferenceTable;
        public short[] CorrectedTable;
        public short[] PotentialTable;

        public rDataRec[] ResearchData;
        public short[] ForwardADC;
        public short[] ReverseADC;


        //--------------------------------------------------------------------
        // constructor for Assay with balnk cal and no tables

        public Assay()
        {
            // constructor logic goes here
        }

        //--------------------------------------------------------------------
        // constructor for blank Assay with specified cal - inits tables to size
        // indicated by cal

        public Assay(Calibration assayCal)
        {
            cal = (Calibration)assayCal.Clone();

            StepCount = (ushort)(Math.Abs(cal.FinalmVolts - cal.DepmVolts) / cal.StepSize + 1);

            ForwardTable = new short[StepCount];
            ReverseTable = new short[StepCount];
            DifferenceTable = new short[StepCount];
            CorrectedTable = new short[StepCount];
            PotentialTable = new short[StepCount];

            ResearchData = new rDataRec[StepCount];
        }

        //--------------------------------------------------------------------
        // deep clone

        public object Clone()
        {
            BinaryFormatter BF = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();

            BF.Serialize(memStream, this);
            memStream.Flush();
            memStream.Position = 0;

            return (BF.Deserialize(memStream));
        }


        //--------------------------------------------------------------------
        // convert time.h format to dateTime for Lab use

        public DateTime AssayTimeMeter
        {
            get
            {
                DateTime assayTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan delta = new TimeSpan(0, 0, 0, Timestamp, 0);

                return assayTime + delta;
            }
            set
            {
            }
        }


        /// <summary>
        /// step number of a given potentail
        /// </summary>
        /// <param name="potential">potential in mV</param>
        /// <returns>step # (the index of potential in all tables</returns>
        public short Step(int potential)
        {
            short step = 0;

            try {
                if (PotentialTable[0] < PotentialTable[StepCount - 1]) {
                    while (step < StepCount && potential > PotentialTable[step])
                        step++;
                } else {
                    while (step < StepCount && potential < PotentialTable[step])
                        step++;
                }

                if (step > StepCount - 1) {
                    step = (short)(StepCount - 1);
                }
            }
            catch (IndexOutOfRangeException) {
            }

            return step;
        }

        //--------------------------------------------------------------------
        // text descriptions of assay errors for display

        public string ErrorString
        {
            get
            {
                string err;
                switch (Error) {
                    case 10: err = "sensor removed too soon"; break;
                    case 11: err = "deposition temp change"; break;
                    case 14: err = "outside temp range"; break;
                    case 15: err = "assay temp change"; break;
                    case 17: err = "current limit exceeded"; break;

                    default: err = "Unknown Error (may be Lc3 specific)"; break;
                }

                return err;
            }
        }
        //--------------------------------------------------------------------
        // read serialized assay XML file to create a new Assay

        /// <summary>
        /// convert full research data into raw averaged adc readings and dispose of extraneous data
        /// </summary>
        public void CollapseResearchData()
        {
            // convert if research data here, otherwise delete field entirely
            if (ResearchData == null || ResearchData[0] == null || ResearchData.Length != StepCount) {
                ResearchData = null;
                return;
            }

            ForwardADC = new short[StepCount];
            ReverseADC = new short[StepCount];

            // accumulate data (get averaged value of last 4 ADC read to get raw fwd & reverse tables)
            const int numSamples = 4;
            for (int step = 0; step < StepCount; step++) {
                int forwardSum = 0;
                int reverseSum = 0;
                for (int i = 1; i <= numSamples; i++) {
                    forwardSum += ResearchData[step].forward[Assay.NumADCreads - i];
                    reverseSum += ResearchData[step].reverse[Assay.NumADCreads - i];
                }
                ForwardADC[step] = (short)(forwardSum / numSamples);
                ReverseADC[step] = (short)(reverseSum / numSamples);
            }

            ResearchData = null;

        }

        //--------------------------------------------------------------------
        // helper function for ConvertFromBinary to fill assay data arrays

        public void CopyArray(ref short[] target, int length, ref int iPos, byte[] binaryData)
        {
            int row = 0;
            target = new short[length];

            while (row < length) {
                target[row++] = BitConverter.ToInt16(binaryData, iPos++ * 2);
            }
            iPos += MaxStepCount - length;
        }


        //--------------------------------------------------------------------
        // to convert from binary stream recieved form meter to Assay 

        public void ConvertFromBinary(byte[] binaryData)
        {
            int i;      	// current integer (2 byte value) positon in binary data stream

            Valid = binaryData[0];
            Error = binaryData[1];

            i = 1;
            CalSignature = BitConverter.ToUInt16(binaryData, i++ * 2);
            StepTime = BitConverter.ToUInt16(binaryData, i++ * 2);
            HalfStepTime = BitConverter.ToUInt16(binaryData, i++ * 2);
            ForwardOffset = BitConverter.ToInt16(binaryData, i++ * 2);
            ReverseOffset = BitConverter.ToInt16(binaryData, i++ * 2);
            StepCount = BitConverter.ToUInt16(binaryData, i++ * 2);
            TempBeforeDep = BitConverter.ToInt16(binaryData, i++ * 2);
            TempAfterDep = BitConverter.ToInt16(binaryData, i++ * 2);
            TempAfterAssay = BitConverter.ToInt16(binaryData, i++ * 2);
            TempAvg = BitConverter.ToInt16(binaryData, i++ * 2);
            LowerMinimum = BitConverter.ToUInt16(binaryData, i++ * 2);
            UpperMinimum = BitConverter.ToUInt16(binaryData, i++ * 2);
            Slope = BitConverter.ToInt16(binaryData, i++ * 2);
            SWCscaled = BitConverter.ToUInt16(binaryData, i++ * 2);

            Timestamp = BitConverter.ToInt32(binaryData, i * 2);
            i += 2;
            SWCValue = BitConverter.ToUInt32(binaryData, i * 2);
            i += 2;
            TempCorrected = BitConverter.ToUInt32(binaryData, i * 2);
            i += 2;
            BLLCorrected = BitConverter.ToUInt32(binaryData, i * 2);
            i += 2;
            Result = BitConverter.ToUInt32(binaryData, i * 2);
            i += 2;

            CopyArray(ref ForwardTable, StepCount, ref i, binaryData);
            CopyArray(ref ReverseTable, StepCount, ref i, binaryData);
            CopyArray(ref DifferenceTable, StepCount, ref i, binaryData);
            CopyArray(ref CorrectedTable, StepCount, ref i, binaryData);
            CopyArray(ref PotentialTable, StepCount, ref i, binaryData);

            ResearchData = new rDataRec[this.StepCount];
        }

        //--------------------------------------------------------------------
        // converts one step record from binary stream to Assay data

        public void ConvertResearchFromBinary(byte[] binaryData, short stepNum)
        {
            int i = 0;

            ResearchData[stepNum] = new rDataRec();

            for (int val = 0; val < ResearchData[stepNum].forward.Length; val++)
                ResearchData[stepNum].forward[val] = BitConverter.ToInt16(binaryData, i++ * 2);

            for (int val = 0; val < ResearchData[stepNum].reverse.Length; val++)
                ResearchData[stepNum].reverse[val] = BitConverter.ToInt16(binaryData, i++ * 2);
        }


        /// <summary>
        /// convert LC3 adc reading to equivalent LC2 reading
        /// </summary>
        /// <param name="lc3reading">ADC reading from LC3 meter</param>
        /// <returns>equivalent ADC reading from LC2</returns>
        private short ConvertLc3AdcReading(short lc3reading)
        {
            int convertedAdc = 2 * lc3reading;

            if (convertedAdc > short.MaxValue)
                convertedAdc = short.MaxValue;
            if (convertedAdc < short.MinValue)
                convertedAdc = short.MinValue;

            return (short)convertedAdc;
        }


        //--------------------------------------------------------------------

        public Assay ReCalcResults()
        {
            Calibration newCal = this.cal.Clone() as Calibration;

            // no need to recal LC3 results, since they were done on PC in first place
            if (Source == Analyzer.LeadCare3) {
                return (Assay)this.Clone();
            }

            // create new assay, set calibration,zero result
            Assay recalc = (Assay)this.Clone();
            recalc.cal = newCal.Clone() as Calibration;
            recalc.Result = 0;
            recalc.SWCValue = 0;

            // only recalc if we have raw data
            if (recalc.ForwardADC == null || recalc.ReverseADC == null || recalc.StepCount == 0) {
                return recalc;
            }

            // intermediate data tables for filtering
            short[] avgForwardTable = new short[recalc.StepCount];
            short[] avgReverseTable = new short[recalc.StepCount];
            short[] rawDiffTable = new short[recalc.StepCount];

            // first fill with raw ADC reads
            for (int step = 0; step < recalc.StepCount; step++) {
                if (Source == Analyzer.LeadCare3) {
                    avgForwardTable[step] = ConvertLc3AdcReading(ForwardADC[step]);
                    avgReverseTable[step] = ConvertLc3AdcReading(ReverseADC[step]);
                } else {
                    avgForwardTable[step] = ForwardADC[step];
                    avgReverseTable[step] = ReverseADC[step];
                }
            }

            TestAgainstCurrentLimit(recalc, avgForwardTable, avgReverseTable);

            // apply boxcar filter to averaged reads to get final forward & reverse tables
            Filter(recalc.ForwardTable, avgForwardTable, BoxcarFilter);
            Filter(recalc.ReverseTable, avgReverseTable, BoxcarFilter);

            // get difference table with ADC overflow flagging and then filter with FFT 
            GetDifferenceCurve(recalc, rawDiffTable);
            Filter(recalc.DifferenceTable, rawDiffTable, FFTfilter);

            // fill raw corrected table and apply baseline correction
            for (int i = 0; i < recalc.StepCount; i++)
                recalc.CorrectedTable[i] = recalc.DifferenceTable[i];
            recalc.BCiterations = 0;
            for (int i = 0; i < 3; i++) {
                if (BaselineCorrection(recalc))
                    ++recalc.BCiterations;
            }

            // integrate to get square wave capacitance
            recalc.SWCValue = 0;
            for (int i = recalc.LowerMinimum; i <= recalc.UpperMinimum; i++) {
                if (recalc.CorrectedTable[i] > 0)
                    recalc.SWCValue += (uint)recalc.CorrectedTable[i];
            }

            // final correction and conversion
            TemperatureCorrection(recalc);
            TranslateBLL(recalc);
            if (recalc.Error == 0) {
                recalc.Result = recalc.BLLCorrected;
            }

            // debug test for calculation
            bool recalcOK = true;
            try {
                for (int step = 0; step < recalc.StepCount; step++) {
                    if (recalc.ForwardTable[step] != this.ForwardTable[step]) recalcOK = false;
                    if (recalc.ReverseTable[step] != this.ReverseTable[step]) recalcOK = false;
                    if (recalc.DifferenceTable[step] != this.DifferenceTable[step]) recalcOK = false;
                    if (recalc.CorrectedTable[step] != this.CorrectedTable[step]) recalcOK = false;
                }
                if (recalc.SWCValue != this.SWCValue) recalcOK = false;
                if (recalc.TempCorrected != this.TempCorrected) recalcOK = false;
                if (recalc.SWCscaled != this.SWCscaled) recalcOK = false;
                if (recalc.BLLCorrected != this.BLLCorrected) recalcOK = false;
            }
            catch (Exception ) {
                recalcOK = false;
            }

            if (recalcOK)
            {
            }

            return recalc;
        }

        /// <summary>
        /// Extrapolated BLL level for SWC values outside BLL table
        /// </summary>
        /// <returns></returns>
        public decimal ExtrapolatedResult()
        {
            decimal exResult = 0;

            if (Result == Assay.LowLead || Result == Assay.HighLead) {
                // declare and init for low lead values (assume origin for first point)
                float swcLow = 0;
                float bllLow = 0;
                float swcHigh = cal.SwcTable[0].swc;
                float bllHigh = cal.SwcTable[0].bll;

                // reset line segment endpoints if swc is too high
                if (Result == Assay.HighLead) {
                    int indexLast = LastSwcIndex(cal);
                    if (indexLast > 0) {
                        swcLow = cal.SwcTable[indexLast - 1].swc;
                        bllLow = cal.SwcTable[indexLast - 1].bll;
                        swcHigh = cal.SwcTable[indexLast].swc;
                        bllHigh = cal.SwcTable[indexLast].bll;
                    }
                } 

                // get scaled SWC value (and handle tmep corrected values over MaxADC)
                float scaledResult = SWCscaled;
                if (SWCscaled == 0) {
                    scaledResult = (65535.0f * TempCorrected) / (float)cal.MaxADCcount;
                }

                // interpolate to obtained corrected BLL 
                float bllResult = (bllHigh - bllLow) / (swcHigh - swcLow);
                bllResult = (scaledResult - swcLow) * bllResult + bllLow;

                // round and convert
                exResult = ((uint)(bllResult + 0.5)) / 10.0M;
            } else {
                // if called with value inside table just return result as decimal
                exResult = Result / 10.0M;
            }

            return exResult;
        }

        //--------------------------------------------------------------------
        // apply filter to raw and return cooked

        private void Filter(short[] cooked, short[] raw, short[] filter)
        {
            int filterCenter = (filter.Length - 1) / 2;
            int startIndex = filterCenter;
            int endIndex = raw.Length - filterCenter;

            // sum of all the filter values
            int filterSum = 0;
            for (int i = 0; i < filter.Length; i++)
                filterSum += filter[i];

            // copy all so that unfiltered data is in destination
            for (int i = 0; i < raw.Length; i++)
                cooked[i] = raw[i];

            // run all values with full raw data thru filter
            for (int step = startIndex; step < endIndex; step++) {
                int sum = 0;
                for (int i = 0; i < filter.Length; i++)
                    sum += raw[step - filterCenter + i] * filter[i];
                cooked[step] = (short)(sum / filterSum);
            }

        }


        //--------------------------------------------------------------------

        private void TestAgainstCurrentLimit(Assay recalc, short[] forward, short[] reverse)
        {
            short ADClimit = 26344;
            bool currentOK = true;

            for (int i = recalc.cal.Min1; i <= recalc.cal.Min4; i++) {
                if (Math.Abs(forward[i]) > ADClimit) currentOK = false;
                if (Math.Abs(reverse[i]) > ADClimit) currentOK = false;
            }

            // 17 is the high current error number
            if (!currentOK)
                recalc.Error = 17;
        }


        /// <summary>
        /// get difference between forward and reverse curves and 
        /// set recal assay error if ADC overflow in resulting table
        /// </summary>
        /// <param name="rawDiffTable">must be same size as Forward & Reverse tables</param>
        private void GetDifferenceCurve(Assay recalc, short[] rawDiffTable)
        {
            for (int i = 0; i < StepCount; i++) {
                int adjustedForward = recalc.ForwardTable[i] - recalc.ForwardTable[recalc.cal.Min1];
                int adjustedReverse = recalc.ReverseTable[i] - recalc.ReverseTable[recalc.cal.Min1];
                int difference = adjustedForward - adjustedReverse;

                if (difference > short.MaxValue) {
                    difference = short.MaxValue;
                    if (i >= recalc.cal.Min1 && i <= recalc.cal.Min4) {
                        recalc.Error = 17;
                    }
                }
                if (difference < short.MinValue) {
                    difference = short.MinValue;
                    if (i >= recalc.cal.Min1 && i <= recalc.cal.Min4) {
                        recalc.Error = 17;
                    }
                }

                rawDiffTable[i] = (short)difference;
            }
        }
        //--------------------------------------------------------------------

        private int IndexOfMinimum(short[] dataArray, int windowLow, int windowHigh)
        {
            int minIndex = windowLow;
            short minData = dataArray[minIndex];

            for (int i = windowLow; i <= windowHigh; i++) {
                if (dataArray[i] <= minData) {
                    minIndex = i;
                    minData = dataArray[minIndex];
                }
            }

            return minIndex;
        }

        //--------------------------------------------------------------------

        private bool BaselineCorrection(Assay recalc)
        {

            int left = IndexOfMinimum(recalc.CorrectedTable, recalc.cal.Min1, recalc.cal.Min2);
            int right = IndexOfMinimum(recalc.CorrectedTable, recalc.cal.Min3, recalc.cal.Min4);

            float rise = recalc.CorrectedTable[right] - recalc.CorrectedTable[left];
            float run = right - left;
            float slope = rise / run;

            recalc.LowerMinimum = (ushort)left;
            recalc.UpperMinimum = (ushort)right;
            recalc.Slope = (short)(slope * 100.0f);

            if ((recalc.CorrectedTable[right] == 0) && (recalc.CorrectedTable[left] == 0)) {
                return false;
            }

            float adjuster = -slope * left;
            short baseline = recalc.CorrectedTable[left];
            int correctedIndex = 0;
            for (int i = 0; i < recalc.StepCount; i++) {
                float temp = recalc.CorrectedTable[i];
                temp = temp - adjuster;
                temp = temp - baseline;

                temp = temp + 0.5f;
                recalc.CorrectedTable[correctedIndex++] = (short)temp;
                adjuster = adjuster + slope;
            }
            return true;
        }

        //--------------------------------------------------------------------

        private void TemperatureCorrection(Assay recalc)
        {
            // get last valid temp in temperature table (table must be monotonic)
            int indexLast = 0;
            while ((indexLast < recalc.cal.TempTable.Length - 1)
                   && (recalc.cal.TempTable[indexLast + 1].temp > recalc.cal.TempTable[indexLast].temp))
                indexLast++;

            // walk thru table to get line segment endpoints
            int index = 1;
            while ((recalc.TempAvg > recalc.cal.TempTable[index].temp) && index < indexLast)
                index++;

            float tempLow = recalc.cal.TempTable[index - 1].temp;
            float tempHigh = recalc.cal.TempTable[index].temp;
            float factorLow = recalc.cal.TempTable[index - 1].factor;
            float factorHigh = recalc.cal.TempTable[index].factor;

            float correctionFactor = (factorHigh - factorLow) / (tempHigh - tempLow);
            correctionFactor = correctionFactor * (recalc.TempAvg - tempLow) + factorLow;

            float result = recalc.SWCValue * (correctionFactor / 1000.0f);
            recalc.TempCorrected = (uint)(result + 0.5f);
        }

        //--------------------------------------------------------------------

        private void TranslateBLL(Assay recalc)
        {

            if (recalc.TempCorrected > recalc.cal.MaxADCcount) {
                recalc.BLLCorrected = Assay.HighLead;
                return;
            }

            ushort scaledSWC = (ushort)((65535.0f * recalc.TempCorrected / recalc.cal.MaxADCcount) + 0.5f);
            recalc.SWCscaled = scaledSWC;

            int indexLast = LastSwcIndex(recalc.cal);

            // if value ouside table bounds then set result and exit
            if (scaledSWC < recalc.cal.SwcTable[0].swc) {
                recalc.BLLCorrected = Assay.LowLead;
                return;
            } else if (scaledSWC >= recalc.cal.SwcTable[indexLast].swc) {
                recalc.BLLCorrected = Assay.HighLead;
                return;
            }

            // walk thru table and get line segment endpoints
            int index = 1;
            while (scaledSWC > recalc.cal.SwcTable[index].swc)
                index++;

            float swcLow = recalc.cal.SwcTable[index - 1].swc;
            float bllLow = recalc.cal.SwcTable[index - 1].bll;
            float swcHigh = recalc.cal.SwcTable[index].swc;
            float bllHigh = recalc.cal.SwcTable[index].bll;

            // interpolate to obtained corrected BLL 
            float bllResult = (bllHigh - bllLow) / (swcHigh - swcLow);
            bllResult = (scaledSWC - swcLow) * bllResult + bllLow;

            // round and convert
            recalc.BLLCorrected = (uint)(bllResult + 0.5);
        }

        /// <summary>
        /// index of last valid entry in SWC (BLL) table of passed calibration
        /// </summary>
        private int LastSwcIndex(Calibration testCal)
        {
            // get last valid entry in BLL table (table must be monotonic)
            int indexLast = 0;
            while ((indexLast < testCal.SwcTable.Length - 1)
                   && (testCal.SwcTable[indexLast + 1].swc > testCal.SwcTable[indexLast].swc)) {
                indexLast++;
            }
            return indexLast;
        }

        // read serialized assay XML file to create a new Assay

        public static Assay Read(string fileName)
        {
            XmlSerializer sr = new XmlSerializer(typeof(Assay));
            FileStream fs = null;
            Assay newAssay;

            try
            {
                fs = File.OpenRead(fileName);
                newAssay = (Assay)sr.Deserialize(fs);

                if (newAssay.Source == Analyzer.LeadCare2 && newAssay.ResearchData != null)
                {
                    newAssay.CollapseResearchData();
                    fs.Close();
                    newAssay.Write(fileName);
                }

                return newAssay;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        //--------------------------------------------------------------------
        // write this to an XML file

        public virtual void Write(string fileName)
        {
            StreamWriter sw = null;
            XmlSerializer sr = new XmlSerializer(typeof(Assay));

            try
            {
                sw = new StreamWriter(fileName);
                sr.Serialize(sw, this);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

    }

}
