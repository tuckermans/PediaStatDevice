using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class QCInfoData
    {
        public QCType _Type;
        public DateTime _Expiration;
        public string _LotCode;
        public UInt16 _Pb_Level1_target;
        public UInt16 _Pb_Level1_range;
        public UInt16 _Pb_Level2_target;
        public UInt16 _Pb_Level2_range;

        public UInt16 _Hgb_Level1_target;
        public UInt16 _Hgb_Level1_range;
        public UInt16 _Hgb_Level2_target;
        public UInt16 _Hgb_Level2_range;
        public UInt16 _Hgb_Level3_target;
        public UInt16 _Hgb_Level3_range;

        public UInt16 MeasureTime;

        public QCInfoData(QCType type, string lotcode, DateTime expiration, UInt16 PbLevel1Target, UInt16 PbLevel1Range, UInt16 PbLevel2Target, UInt16 PbLevel2Range)
        {
            _Type = type;
            _LotCode = lotcode;
            _Expiration = expiration;
            _Pb_Level1_target = PbLevel1Target;
            _Pb_Level1_range = PbLevel1Range;
            _Pb_Level2_target = PbLevel2Target;
            _Pb_Level2_range = PbLevel2Range;

            MeasureTime = 180;

        }


        public QCInfoData(QCType type, string lotcode, DateTime expiration, UInt16 PbLevel1Target, UInt16 PbLevel1Range, UInt16 PbLevel2Target, UInt16 PbLevel2Range,
             UInt16 HgbLevel1Target, UInt16 HgbLevel1Range, UInt16 HgbLevel2Target, UInt16 HgbLevel2Range, UInt16 HgbLevel3Target, UInt16 HgbLevel3Range)
        {
            _Type = type;
            _LotCode = lotcode;
            _Expiration = expiration;


            _Pb_Level1_target = PbLevel1Target;
            _Pb_Level1_range = PbLevel1Range;
            _Pb_Level2_target = PbLevel2Target;
            _Pb_Level2_range = PbLevel2Range;

            _Hgb_Level1_target = HgbLevel1Target;
            _Hgb_Level1_range = HgbLevel1Range;
            _Hgb_Level2_target = HgbLevel2Target;
            _Hgb_Level2_range = HgbLevel2Range;
            _Hgb_Level3_target = HgbLevel3Target;
            _Hgb_Level3_range = HgbLevel3Range;

            MeasureTime = 180;

        }

        public int Marshall( ref byte[] Payload )
        {
            ushort idx = 3;

            // Button Type
            Payload[idx] = (byte)_Type;      
            idx += 1;

            //ushort day=5, month=11, year=2015;
            SerialMessage.SplitWord((ushort)_Expiration.Day, ref Payload, idx);
            idx += 2;

            SerialMessage.SplitWord((ushort)_Expiration.Month, ref Payload, idx);
            idx += 2;

            SerialMessage.SplitWord((ushort)_Expiration.Year, ref Payload, idx);
            idx += 2;

            byte[] ascii = Encoding.ASCII.GetBytes(_LotCode);
            foreach (Byte b in ascii)
            {
                Payload[idx++] = b;
            }

           // idx += (short)Constants.LOT_LEN; // 12

            SerialMessage.SplitWord(MeasureTime, ref Payload, idx);
            idx += 2;

            // target values and ranges

            // Level 1
            SerialMessage.SplitWord(_Pb_Level1_target, ref Payload, idx);
            idx += 2;

            SerialMessage.SplitWord(_Pb_Level1_range, ref Payload, idx);
            idx += 2;

            // Level2
            SerialMessage.SplitWord(_Pb_Level2_target, ref Payload, idx);
            idx += 2;

            SerialMessage.SplitWord(_Pb_Level2_range, ref Payload, idx);
            idx += 2;

            if (_Type == QCType.QC_PB_HGB)
            {
                // Level 1
                SerialMessage.SplitWord(_Hgb_Level1_target, ref Payload, idx);
                idx += 2;

                SerialMessage.SplitWord(_Hgb_Level1_range, ref Payload, idx);
                idx += 2;

                // Level2
                SerialMessage.SplitWord(_Hgb_Level2_target, ref Payload, idx);
                idx += 2;

                SerialMessage.SplitWord(_Hgb_Level2_range, ref Payload, idx);
                idx += 2;

                // Level3
                SerialMessage.SplitWord(_Hgb_Level3_target, ref Payload, idx);
                idx += 2;

                SerialMessage.SplitWord(_Hgb_Level3_range, ref Payload, idx);
                idx += 2;
            }
            return idx;

        } // Marshall()
       
    } // class
} // namespace
