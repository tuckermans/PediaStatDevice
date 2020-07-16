using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;



namespace PediaStatDevice
{
    

    public enum CmdIDType: byte
    {
        ACK,
        NAK,
        GET_VERSION,
        GET_STATUS,
        GET_CAL,
        SET_CAL,
        GET_TIME,
        SET_TIME,
        GET_ASSAY,
        SET_ASSAY,
        GET_RDATA,
        SET_RDATA,
        CLR_ASSAY,

        GET_POST_LOG,
        GET_EVENT_LOG,
        CLEAR_LOGS,

        GET_IO_LINE,
        SET_IO_LINE,
        GET_ADC,
        GET_TEMP,
        SET_DAC,
        SET_BUZZER,
        SET_LCD,
        SET_PRINTER,

        LOCK_METER,
        RESET_METER,

        SET_INJECTED,
        RUN_ASSAY,

        SET_EMR,
        SET_SAMPLEID,
        GET_RESULT,
        SET_SENSOR_TIMEOUT,

        SET_NEW_SENSOR,
        GET_NEW_ASSAY,
        SET_THB_DAC,
        GET_THB_ADC,
        SET_NEW_ASSAY,
        GET_NG_CAL,
        GET_NG_ASSAY,

        GET_LAST_SAMPLE,
        GET_PENDING_SAMPLE,
        GET_PENDING_QC,
        // New Diag Mode
        START_DIAG_MODE,
        END_DIAG_MODE,
        SENSOR_DECTECTED,
        SENSOR_REMOVED,
        QC_INFO,
        TEMP_HIGH,
        TEMP_LOW,
        TEMP_OK,
        TEMP_UNSTABLE,
        SENSOR_OK,
        MEASURE_COMPLETE,
        MEASURE_CANCELLED,
        SAMPLE_DETECTED,
        MEASURED_RESULTS,
        USB_BARCODE,
        BT_CONNECT,
        BT_DISCONNECT,
        BT_CONNECT_FAILED,
        BT_BARCODE_DATA,
        BT_SCAN_REPORT,
        // Diagnostic Commands
        GET_SENSOR_STATUS,
        GET_SENSOR_ID,
        GET_DAC_STATUS,
        SET_DAC_STATUS,
        SET_LED_STATUS,
        SET_A2D_STATUS,
        GET_A2D_STATUS,
        SET_SWITCH_STATE,
        GET_SWITCH_STATUS,
        SET_CHAN1_GAIN,
        SET_CHAN2_GAIN,
        GET_CHAN1_GAIN,
        GET_CHAN2_GAIN,
        SELECT_LED,
        RUN_POST,
        RESET_STORAGE,
        GET_CONFIG,
        SET_CONFIG,
        SET_DISPLAY,
        GET_QC_DATA,
        GET_BUTTON_DATA,
        SET_SERIAL_NUM,
        GET_ACTIVE_LOT,
        GET_LAST_QC_SAMPLE,
        CLR_CAL_STATUS,
        // DMS Commands
        GET_FIRST_PATIENT_RESULT,
        GET_NEXT_PATIENT_RESULT,
        GET_FIRST_QC_RESULT,
        GET_NEXT_QC_RESULT,
        GET_FIRST_BUTTON,
        GET_NEXT_BUTTON,
        MARK_RECORD_SENT,
        CLEAR_LOG,
        SET_OPER_LIST,
        GET_OPTICAL_CAL,
        RESET_OPTICAL_CAL,
        GET_MFG_DATA_LOG,
        LAST_CMD
    }

    public enum RecordTypeEnum : byte
    {
        PATIENT = 0x0,
        QC      = 0x1,
        BUTTON  = 0x2,
        POST    = 0x3,
        EVENT   = 0x4,
        CAL     = 0x5,
        MFG     = 0x6,
    }
    /// <summary>
    /// Actual ASCII control character mappings
    /// </summary>
    public enum ControlCode : byte
    {
        NUL, SOH, STX, ETX, 
        EOT, ENQ, ACK, BEL, 
        BS, HT, LF, VT, 
        FF, CR, SO, SI, 
        DLE, DC1, DC2, DC3, 
        DC4, NAK, SYN, ETB, 
        CAN, EM, BUS, ESC, 
        FS, GS, RS, US
    }

    /// <summary>
    /// Simple entity class to encapsualte the command id and the text of the command
    /// </summary>
    public class LcCommand
    {
        public CmdIDType Command { get; private set; }
        public string Text { get; private set; }

        public LcCommand(CmdIDType cmd, string txt)
        {
            Command = cmd;
            Text = txt;
        }
    }

    public class LcProtocol
    {
        public static readonly float MIN_VERSION = 1.06f;
        private Crc16 _crc = new Crc16();

        public List<LcCommand> QueryCommands { get; private set; }

        public LcProtocol()
        {
            BuildQueryCommandList();
        }

        private void BuildQueryCommandList()
        {
            QueryCommands = new List<LcCommand>();
            QueryCommands.Add(new LcCommand(CmdIDType.GET_ADC, "Get ADC values"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_ASSAY, "Get assay"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_VERSION, "Get version"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_STATUS, "Get status"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_CAL, "Get calibration values"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_TIME, "Get time"));
            QueryCommands.Add(new LcCommand(CmdIDType.SET_TIME, "Set time"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_RDATA, "Get research data"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_POST_LOG, "Get post log"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_EVENT_LOG, "Get event log"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_IO_LINE, "Get IO line"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_TEMP, "Get temp"));
            QueryCommands.Add(new LcCommand(CmdIDType.SET_EMR, "Set EMR mode"));
            QueryCommands.Add(new LcCommand(CmdIDType.SET_SAMPLEID, "Set sample ID"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_RESULT, "Get last result"));
            QueryCommands.Add(new LcCommand(CmdIDType.SET_SENSOR_TIMEOUT, "Set sensor remove"));

            QueryCommands.Add(new LcCommand(CmdIDType.SET_NEW_SENSOR, "Set new sensor parameters"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_NEW_ASSAY, "Get new sensor data"));
            QueryCommands.Add(new LcCommand(CmdIDType.SET_THB_DAC, "Set tHb DAC"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_THB_ADC, "Get tHb ADC values"));
            QueryCommands.Add(new LcCommand(CmdIDType.GET_LAST_SAMPLE, "Get last patient sample"));
            //QueryCommands.Add(new LcCommand(CmdIDType.GET_PENDING_SAMPLES, "Get pending sample results"));
        }

        public byte[] CreateMessage(CmdIDType cmd, string data)
        {
            if (data != null)
            {
                return CreateMessage(cmd, Encoding.ASCII.GetBytes(data));
            }
            else
            {
                return CreateMessage(cmd);
            }
        }

        public byte[] CreateMessage(CmdIDType cmd, byte[] data = null)
        {
            MemoryStream strm = new MemoryStream();

            strm.WriteByte((byte)cmd);
            ushort length = data == null ? (ushort) 0 : (ushort)data.Length;
            byte[] val = BitConverter.GetBytes(length);
            strm.Write(val, 0, val.Length);
            if (data != null)
            {
                strm.Write(data, 0, data.Length);
            }

            ushort crc = _crc.ComputeChecksum(strm.GetBuffer());
            val = BitConverter.GetBytes(crc);
            strm.Write(val, 0, val.Length);
            return strm.GetBuffer();
        }

        public bool ValidateMessage(byte[] msg)
        {
            ushort calcCRC = _crc.ComputeChecksum(msg, 0, msg.Length - 2);
            //ushort recvCRC = (ushort)((msg[msg.Length - 1] << 8) | (msg[msg.Length - 2]));
            ushort recvCRC = BitConverter.ToUInt16(msg, msg.Length - 2);

            return (calcCRC == recvCRC);
        }
    }
}
