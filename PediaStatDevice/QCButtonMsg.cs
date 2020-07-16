using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public enum ButtonTypeEnum : byte
    {
        PBOnly,
        PbHgb,
        Bilirubin,
        OptVerifier,
        Cholesterol,
        spare5,
        spare6,
        ElectTest,
        spare8,
        spare9,
        spare10,
        spare11,
        spare12,
        spare13,
        spare14,
        spare15,
    }
    public class QCButtonMsg : SerialMessage
    {
        public QCButtonData QCInfo = new QCButtonData();

        public QCButtonMsg(byte[] data, uint len)
            : base(CmdIDType.GET_QC_DATA, data, len)
        {
            // Lot Code
            QCInfo.LotCode = GetString(0, 6);
           
            // button type
            ButtonTypeEnum buttonType = (ButtonTypeEnum)GetDWord();

            // Expiration Date
            // skip time
            Skip(12);
                        
            int Month = (int)GetDWord();            
            int Day = (int)GetDWord();            
            int Year = (int)GetDWord();
                         
            QCInfo.Expiration = new DateTime(Year,Month,Day);

            // skip PM Flag
            Skip(4);
                      
            // Measure Time
            QCInfo.MeasureTime = GetWord();

            ////////////////////////////////////////
            // Channel 1
            // Level 1 TargetValue
            // Skip level                        
            Skip(4);
            QCInfo.Chan1Level1 = new QCRanges();
            QCInfo.Chan1Level1.Target = ((float)GetWord()) / 10;
            QCInfo.Chan1Level1.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel1.Add(QCInfo.Chan1Level1);

            // Level 2 TargetValue
            // Skip level                        
            Skip(4);
            QCInfo.Chan1Level2 = new QCRanges();
            QCInfo.Chan1Level2.Target = ((float)GetWord()) / 10;
            QCInfo.Chan1Level2.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel2.Add(QCInfo.Chan1Level2);

            // Level 3 TargetValue
            // Skip level
            Skip(4);            
            QCInfo.Chan1Level3 = new QCRanges();
            QCInfo.Chan1Level3.Target = ((float)GetWord()) / 10;
            QCInfo.Chan1Level3.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel3.Add(QCInfo.Chan1Level3);

            ////////////////////////////////////////
            // Channel 2
            // Level 1 TargetValue
            // Skip level
            Skip(4);
            QCInfo.Chan2Level1 = new QCRanges();
            QCInfo.Chan2Level1.Target = ((float)GetWord()) / 10;
            QCInfo.Chan2Level1.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel1.Add(QCInfo.Chan2Level1);
            
            // Level 2 TargetValue
            // Skip level
            Skip(4);
            QCInfo.Chan2Level2 = new QCRanges();
            QCInfo.Chan2Level2.Target = ((float)GetWord()) / 10;
            QCInfo.Chan2Level2.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel2.Add(QCInfo.Chan2Level2);

            // Level 3 TargetValue
            // Skip level
            Skip(4);
            QCInfo.Chan2Level3 = new QCRanges();
            QCInfo.Chan2Level3.Target = ((float)GetWord()) / 10;
            QCInfo.Chan2Level3.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel3.Add(QCInfo.Chan2Level3);

            ////////////////////////////////////////
            // Channel 3
            // Level 1 TargetValue
            // Skip level
            Skip(4);
            QCInfo.Chan3Level1 = new QCRanges();
            QCInfo.Chan3Level1.Target = ((float)GetWord()) / 10;
            QCInfo.Chan3Level1.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel1.Add(QCInfo.Chan3Level1);

            // Level 2 TargetValue
            // Skip level
            Skip(4);
            QCInfo.Chan3Level2 = new QCRanges();
            QCInfo.Chan3Level2.Target = ((float)GetWord()) / 10;
            QCInfo.Chan3Level2.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel2.Add(QCInfo.Chan3Level2);

            // Level 3 TargetValue
            // Skip level
            Skip(4);
            QCInfo.Chan3Level3 = new QCRanges();
            QCInfo.Chan3Level3.Target = ((float)GetWord()) / 10;
            QCInfo.Chan3Level3.Range = ((float)GetWord()) / 10;
            QCInfo.QCLevel3.Add(QCInfo.Chan3Level3);

            // Measurable Ranges
            QCInfo.Chan1Ranges = new Range(0, 0);
            QCInfo.Chan1Ranges.LowerLimit = ((float)GetWord()) / 10;
            QCInfo.Chan1Ranges.UpperLimit = ((float)GetWord()) / 10;

            QCInfo.Chan2Ranges = new Range(0, 0);
            QCInfo.Chan2Ranges.LowerLimit = ((float)GetWord()) / 10;
            QCInfo.Chan2Ranges.UpperLimit = ((float)GetWord()) / 10;

            QCInfo.Chan3Ranges = new Range(0, 0);
            QCInfo.Chan3Ranges.LowerLimit = ((float)GetWord()) / 10;
            QCInfo.Chan3Ranges.UpperLimit = ((float)GetWord()) / 10;

            Skip(2);    // skip Out Of Vial Time
            Skip(4);    // Skip Sent Flag

            QCInfo.SensorID = (SensorIDEnum)GetWord();
            QCInfo.LotCRC = (ushort)GetWord();
            QCInfo.timestamp = GetDWord();
            QCInfo.timeOffsetMins = GetWord();

            short crc = GetWord();
        }
    } // class
}
