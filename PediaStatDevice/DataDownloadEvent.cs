using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PediaStatDevice
{

    public class CommandResultEvent : EventArgs
    {
        public bool Result
        {
            get;
            set;
        }

        public CmdIDType Command
        {
            get;
            set;
        }

        public CommandResultEvent(CmdIDType cmd)
        {
            Command = cmd;
           
        }

        public CommandResultEvent(CmdIDType cmd, LcDataPacket packet) : this(cmd)
        {
            if (null != packet && packet.Data != null)
            {
                Result = (packet.Data[0] == 0x01) ? true : false;
            }

        }
    }

    public class PostExecuteResult : CommandResultEvent
    {
        public int ResultCode
        {
            get;
            private set;
        }
        public PostExecuteResult(LcDataPacket packet) : base(CmdIDType.RUN_POST,packet)
        {
            if (null != packet && packet.Data != null)
            {
                ResultCode = packet.Data[0] | (packet.Data[1] << 8);
            }
        }
    }
    public class DataDownloadEvent : CommandResultEvent
    {
        public bool OpenEnabled
        {
            get;
            set;
        }
        public bool Status
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public String Serial
        {
            get;
            set;
        }
        public DataDownloadEvent(CmdIDType cmd, bool status)
            : base(cmd, null)
        {
            Status = status;
        }

        public DataDownloadEvent(CmdIDType cmd, int count, bool openOption=false)
            : base(cmd, null)
        {
            Count = count;
            OpenEnabled = openOption;
        }

        public DataDownloadEvent(CmdIDType cmd)
            : base(cmd, null)
        {
            Status = true;
        }
    }
    public class MFGDataLogDownloadEvent : DataDownloadEvent
    {
        public List<MFGLog> Log
        {
            get;
            set;
        }
        public MFGDataLogDownloadEvent(int count, bool openOption = false)
            : base(CmdIDType.GET_MFG_DATA_LOG, count, openOption)
        {
        }

        public MFGDataLogDownloadEvent(List<MFGLog> log)
            : base(CmdIDType.GET_MFG_DATA_LOG, log.Count)
        {
            Log = log;
        }

    }
    public class POSTLogDownloadEvent : DataDownloadEvent
    {
        public List<POSTLog> Log
        {
            get;
            set;
        }
        public POSTLogDownloadEvent(int count, bool openOption=false)
            : base(CmdIDType.GET_POST_LOG, count, openOption)
        {
        }

        public POSTLogDownloadEvent(List<POSTLog> log)
            : base(CmdIDType.GET_POST_LOG, log.Count)
        {
            Log = log;         
        }

    }
    public class EventLogDownloadEvent : DataDownloadEvent
    {
        public List<EventLog> Log
        {
            get;
            set;
        }
        public EventLogDownloadEvent(int count, bool Option=false)
            : base(CmdIDType.GET_EVENT_LOG, count, Option)
        {
        }

        public EventLogDownloadEvent(List<EventLog> log)
            : base(CmdIDType.GET_EVENT_LOG, log.Count)
        {
            Log = log;
        }

    }

    public class PatientSampleEvent : DataDownloadEvent
    {
     

        public List<PatientResult> Results
        {
            get;
            set;
        }

        public PatientSampleEvent(int count, bool Option=false)
            : base(CmdIDType.GET_PENDING_SAMPLE, count, Option)
        {
        }

        public PatientSampleEvent(List<PatientResult> list)
            : base(CmdIDType.GET_PENDING_SAMPLE, list.Count)
        {
            Results = list;
        }

    }

    public class QCSampleEvent : DataDownloadEvent
    {
        public List<QCResult> Results
        {
            get;
            set;
        }
        public QCSampleEvent(int count, bool Option=false)
            : base(CmdIDType.GET_PENDING_QC, count, Option)
        {
        }

        public QCSampleEvent(List<QCResult> list)
            : base(CmdIDType.GET_PENDING_QC, list.Count)
        {
            Results = list;
        }
    }

    public class VersionEvent : CommandResultEvent
    {
        private short CtlMajor, CtlMinor, CtlBuild;
        private short UIMajor, UIMinor, UIBuild;

        public string Version
        {
            get
            {
                return string.Format("UI:  {0}.{1}.{2}    RT:  {3}.{4}.{5}\n", UIMajor, UIMinor, UIBuild, CtlMajor, CtlMinor, CtlBuild);
            }
        }

        public DateTime Time
        {
            get;
            set;
        }
        public VersionEvent(LcDataPacket packet)
            : base(CmdIDType.GET_VERSION, null)
        {
            UIMajor = SerialMessage.PackWord(packet.Data, 0);
            UIMinor = SerialMessage.PackWord(packet.Data, 2);
            UIBuild = SerialMessage.PackWord(packet.Data, 4);

            CtlMajor = SerialMessage.PackWord(packet.Data, 6);
            CtlMinor = SerialMessage.PackWord(packet.Data, 8);
            CtlBuild = SerialMessage.PackWord(packet.Data, 10);
        }

        public VersionEvent(byte[] Data) :  base(CmdIDType.GET_VERSION)
        {
            UIMajor = SerialMessage.PackWord(Data, 0);
            UIMinor = SerialMessage.PackWord(Data, 2);
            UIBuild = SerialMessage.PackWord(Data, 4);

            CtlMajor = SerialMessage.PackWord(Data, 6);
            CtlMinor = SerialMessage.PackWord(Data, 8);
            CtlBuild = SerialMessage.PackWord(Data, 10);

        }
    }
    /// <summary>
    /// Event sent in response to a Patient Sample download request.
    /// </summary>
    public class ButtonDownloadEvent : DataDownloadEvent
    {
        public List<QCButtonData> Results
        {
            set;
            get;
        }

        public ButtonDownloadEvent(bool status)
            : base(CmdIDType.GET_BUTTON_DATA, status)
        {
        }

        public ButtonDownloadEvent(List<QCButtonData> r, bool Option=false)
            : base(CmdIDType.GET_BUTTON_DATA,r.Count, Option)
        {
            Results = r;
        }


    }

    /// <summary>
    /// Optical Calibration Data
    /// </summary>
    public class OpticalCalEvent : CommandResultEvent
    {
        private UInt16 target;
        private LedDataInfo[] ledData = new LedDataInfo[8];
        private UInt16 CRC;

        /// <summary>
        /// Target value
        /// </summary>
        public UInt16 Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
            }
        }

        /// <summary>
        /// Array of LED Data Info
        /// </summary>
        public LedDataInfo[] LedData
        {
            get
            {
                return ledData;
            }
        }

        public OpticalCalEvent(LcDataPacket packet)
            : base(CmdIDType.GET_OPTICAL_CAL, null)
        {
            target = (UInt16)SerialMessage.PackWord(packet.Data, 0);

            int idx = 2;
            // 8 LED Data Info
            for (int i = 0; i < 8; i++, idx +=18)
            {
                ledData[i] = new LedDataInfo(packet.Data, idx);
                ledData[i].LED = i;
            }
            //idx += 14;
            CRC = (UInt16)SerialMessage.PackWord(packet.Data, idx);
        }
       
    } // class

    public class LedDataInfo 
    {
        private UInt16 response_Low_mA;
        private UInt16 response_High_mA;
        private float slope;
        private float intercept;
        private UInt16 lowCurrent;
        private UInt16 highCurrent;
        private UInt16 nomCurrent;

        public int LED
        {
            get;
            set;
        }

        public UInt16 ResponseAtLow_mA
        {
            get
            {
                return response_Low_mA;
            }
        }

        public UInt16 ResponseAtHigh_mA
        {
            get
            {
                return response_High_mA;
            }
        }

        public float Slope
        {
            get
            {
                return slope;
            }
        }

        public float Intercept
        {
            get
            {
                return intercept;
            }
        }

        public UInt16 LowCurrent
        {
            get
            {
                return lowCurrent;
            }
        }

        public UInt16 HighCurrent
        {
            get
            {
                return highCurrent;
            }
        }

        public UInt16 NomCurrent
        {
            get
            {
                return nomCurrent;
            }
        }

        public LedDataInfo(byte[] data, int start)
        {
            response_Low_mA = (UInt16)SerialMessage.PackWord(data, start); start += 2;
            response_High_mA = (UInt16)SerialMessage.PackWord(data, start); start += 2;
            slope = (float)SerialMessage.GetFloat(data, start); start += 4;
            intercept = (float)SerialMessage.GetFloat(data, start); start += 4;
            lowCurrent = (UInt16)SerialMessage.PackWord(data, start); start += 2;
            highCurrent = (UInt16)SerialMessage.PackWord(data, start); start += 2;            
            nomCurrent = (UInt16)SerialMessage.PackWord(data, start); start += 2;            
        }
    } // class

    /// <summary>
    /// Result of Optical Calibration
    /// </summary>
    public class ResetOpticalResult : CommandResultEvent
    {
        public int ResultCode
        {
            get;
            private set;
        }
        public ResetOpticalResult(LcDataPacket packet)
            : base(CmdIDType.RESET_OPTICAL_CAL, packet)
        {
            if (null != packet && packet.Data != null)
            {
                ResultCode = packet.Data[0] | (packet.Data[1] << 8);
            }
        }
    }
}
