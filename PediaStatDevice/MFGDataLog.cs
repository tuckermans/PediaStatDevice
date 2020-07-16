using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class MFGLog
    {
        public string serialNumber;
        public UInt32 numSerialNumberUpdates;
        public UInt16 UIVerMajor;
        public UInt16 UIVerMinor;
        public UInt16 UIVerBuild;
        public UInt32 numUIFirmwareUpdates;
        public UInt16 RTVerMajor;
        public UInt16 RTVerMinor;
        public UInt16 RTVerBuild;
        public UInt32 numRTFirmwareUpdates;
        public string BT_FirmwareVersion;
        public int masterResetDateTime;
        public UInt32 numMasterResets;
        public int mfgResetDateTime;
        public UInt32 numMfgResets;
        public UInt32 numOnTimeMins;
        public UInt32 numOnTimeMinsSinceMfgReset;
        public UInt32[] events = new UInt32[25];
        public UInt32 totalNonFatalSystemErrors;
        public UInt32 totalFatalSystemErrors;
        public UInt32[] nonFatalErrors = new UInt32[50];
        public UInt32[] fatalErrors = new UInt32[50];

        public UInt16 crc;                        // record CRC (MUST be last field in struct)

        public string ToCSV
        {
            get
            {
                string masterResetTime;
                masterResetTime = UnixTime.FromUnixTime(masterResetDateTime).ToString("yyyy-MM-ddTHH:mm:ss");

                string mfgResetTime;
                mfgResetTime = UnixTime.FromUnixTime(mfgResetDateTime).ToString("yyyy-MM-ddTHH:mm:ss");

                StringBuilder builder = new StringBuilder();

                int eventIndex = 1;
                int nonFatalIndex = 1;
                int fatalIndex = 1;

                builder.Append("Description, Value, Count\n")
                    .Append("SerialNumber:,").Append(serialNumber).Append(",").Append(numSerialNumberUpdates).Append("\n")
                    .Append("UI FW Rev:,").Append(UIVerMajor).Append(".").Append(UIVerMinor).Append(".").Append(UIVerBuild).Append(",").Append(numUIFirmwareUpdates).Append("\n")
                    .Append("RT FW Rev:,").Append(RTVerMajor).Append(".").Append(RTVerMinor).Append(".").Append(RTVerBuild).Append(",").Append(numRTFirmwareUpdates).Append("\n")
                    .Append("RM FW Rev:,").Append(BT_FirmwareVersion).Append(",").Append("\n")
                    .Append("MasterResetDateTime:,").Append(masterResetTime).Append(",").Append(numMasterResets).Append("\n")
                    .Append("MfgResetDateTime:,").Append(mfgResetTime).Append(",").Append(numMfgResets).Append("\n")
                    .Append("InstrumentOnTime (mins):,").Append(numOnTimeMins).Append(",").Append("\n")
                    .Append("InstrumentOnTimeSinceLastMfgReset (mins):,").Append(numOnTimeMinsSinceMfgReset).Append("\n\n")

                    // .Append("NULL_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("PB_ASSAY_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("ERROR_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("POST_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("CAL_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("ASSERT_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("HGB_ASSAY_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("HCT_ASSAY_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("LOGON_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("LOGOFF_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("LOGON_FAILED,,").Append(events[eventIndex++]).Append("\n")
                    .Append("TIME_CHANGED_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("DATE_CHANGED_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("PB_QC_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("HGB_QC_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("BILI_ASSAY_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("BILI_QC_EVENT,,").Append(events[eventIndex++]).Append("\n")
                    .Append("OVERCURRENT_EVENT,,").Append(events[eventIndex++]).Append("\n\n")

                    .Append("Non-Fatal System Errors,,").Append(totalNonFatalSystemErrors).Append("\n")
                    .Append("Fatal Errors,,").Append(totalFatalSystemErrors).Append("\n\n")

                    //.Append("ERR_NONE,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_TEMP_RANGE,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_TEMP_UNSTABLE_WAIT,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_SENSOR_LOT_OLD,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_5,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_SENSOR_WET,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_OUT_OF_VIAL,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_8,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_WETTING,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_SENSOR_REMOVED,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_DEP_TEMP_CHANGE,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_12,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_13,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_ASSAY_TEMP_RANGE,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_ASSAY_TEMP_CHANGE,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_16,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_HIGH_CUR,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_18,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_ISEP_SENSOR_ERROR,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_20,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_21,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_22,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_BUTTON_UNK,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_24,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_UNSUPPORTED_BUTTON,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_INVALID_BUTTON,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_27,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_28,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_29,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_INTERNAL_STORAGE_ERROR,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_INVALID_GAIN_SELECT,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_INVALID_ASSAY_PARAMETER,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_INVALID_OPTICAL_ASSAY_PARAMETER,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_SENSOR_TYPE_MISMATCH,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_35,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_OPTICAL_BLANK_OOR,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_OPTICAL_EMPTY_OOR,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_SENSOR_INSERTION_TOO_FAST,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_OPTICAL_FILLED_OOR,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_BAD_PASSWORD,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_BT_FAILURE,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_DACA_OOR_DURING_DEPO,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_UNDILUTED_SAMPLE_DETECTED,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_BILIRUBIN_MEAS_ERROR,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_RTC_FAILURE,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n")
                    .Append("ERR_SENSOR_SHORT_DETECTED,,").Append(nonFatalErrors[nonFatalIndex++]).Append("\n\n")

                    //.Append("FIRST_FATAL_ERROR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1001,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1002,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_BLOCK_TEMP_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_DACA_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1005,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_MEAS1_ZERO_CURRENT_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1007,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1008,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1009,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_FBK1_SINGLE_ELECTRODE_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1011,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_MEAS1_SINGLE_ELECTRODE_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_FBK1_DUMMY_ELECTRODE_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1014,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_MEAS1_DUMMY_ELECTRODE_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1016,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1017,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_NOT_USED_1018,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_INTERNAL_ERROR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_COMMS_FAILURE,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_MAX_TIMERS,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_UI_MEMORY_CHECK_ERROR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_CTL_MEMORY_CHECK_ERROR,,").Append(fatalErrors[fatalIndex++]).Append("\n")

                    .Append("ERR_OPTICAL_INTENSITY_OFF_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_OPTICAL_INTENSITY_NOMINAL_CURRENT_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_OPTICAL_INTENSITY_HIGH_CURRENT_OOR,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_FAN_NOT_RUNNING_FAILURE,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_OPTICAL_CALIBRATION_FAILURE,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_TOUCHSCREEN_STARTUP_FAILURE,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_RTC_STARTUP_FAILURE,,").Append(fatalErrors[fatalIndex++]).Append("\n")
                    .Append("ERR_ELECTRODE_SHORT_DETECTED,,").Append(fatalErrors[fatalIndex++]).Append("\n\n");

                return builder.ToString();
            }

        }

        public MFGLog(byte[] data)
        {
            int idx = 0;
            serialNumber = Encoding.ASCII.GetString(data, idx, 12);
            idx += 12;

            numSerialNumberUpdates = (UInt32) SerialMessage.PackDWord(data, idx);
            idx += 4;

            UIVerMajor = (UInt16) SerialMessage.PackWord(data, idx);
            idx += 2;

            UIVerMinor = (UInt16) SerialMessage.PackWord(data, idx);
            idx += 2;

            UIVerBuild = (UInt16) SerialMessage.PackWord(data, idx);
            idx += 2;

            numUIFirmwareUpdates = (UInt32) SerialMessage.PackDWord(data, idx);
            idx += 4;

            RTVerMajor = (UInt16) SerialMessage.PackWord(data, idx);
            idx += 2;

            RTVerMinor = (UInt16) SerialMessage.PackWord(data, idx);
            idx += 2;

            RTVerBuild = (UInt16) SerialMessage.PackWord(data, idx);
            idx += 2;

            numRTFirmwareUpdates = (UInt32) SerialMessage.PackDWord(data, idx);
            idx += 4;

            BT_FirmwareVersion = Encoding.ASCII.GetString(data, idx, 30);
            idx += 30;

            masterResetDateTime = (int) SerialMessage.PackDWord(data, idx);
            idx += 4;
            
            numMasterResets = (UInt32) SerialMessage.PackDWord(data, idx);
            idx += 4;
            
            mfgResetDateTime = (int) SerialMessage.PackDWord(data, idx);
            idx += 4;
            
            numMfgResets = (UInt32) SerialMessage.PackDWord(data, idx);
            idx += 4;
            
            numOnTimeMins = (UInt32) SerialMessage.PackDWord(data, idx);
            idx += 4;
            
            numOnTimeMinsSinceMfgReset = (UInt32) SerialMessage.PackDWord(data, idx);
            idx += 4;

            // CRC Of Manufacturing Info
            crc = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;

            for (int i = 0; i < 25; i++)
            {
                events[i] = (UInt32)SerialMessage.PackDWord(data, idx);
                idx += 4;
            }

            totalNonFatalSystemErrors = (UInt32)SerialMessage.PackDWord(data, idx);
            idx += 4;

            totalFatalSystemErrors = (UInt32)SerialMessage.PackDWord(data, idx);
            idx += 4;

            for (int i = 0; i < 50; i++)
            {
                nonFatalErrors[i] = (UInt32)SerialMessage.PackDWord(data, idx);
                idx += 4;
            }

            for (int i = 0; i < 50; i++)
            {
                fatalErrors[i] = (UInt32)SerialMessage.PackDWord(data, idx);
                idx += 4;
            }

            // CRC Of Manufacturing Counter Data
            crc = (UInt16)SerialMessage.PackWord(data, idx);
            idx += 2;

            // CRC Of Entire Manufacturing Data Log
            crc = (UInt16)SerialMessage.PackWord(data, idx);
        }
    }
}
