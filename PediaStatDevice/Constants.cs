using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public enum MessageType
    {
        EVENT_TYPE = 100,
        COMMAND_TYPE,
        EVENT_DATA_TYPE,
        ERROR_TYPE,
        KEYBOARD_MSG,
        TIMER_MSG,
        TOUCH_TYPE,
        BLUETOOTH_MSG,
        STATE_TYPE,
    }

    // Events: Most events are sent by the UI to FSM to
    // trigger actions
    public enum MessageEventEnum
    {
        FIRST_EVENT = 200,
        EV_UNKNOWN_EVENT,
        EV_TIMEOUT,
        EV_TIMER_250MS_TICK,
        EV_NEW_QC_LOT,
        EV_QC_LOT_EXPIRED,
        EV_BTN_OK,
        EV_BTN_CANCEL,
        EV_BTN_YES,
        EV_BTN_NO,
        EV_BTN_SAVE,
        EV_BTN_CFG_FAST_CHARGE, // 210
        EV_BTN_CONFIG,
        EV_BTN_REVIEW,
        EV_DONE,
        EV_REVIEW_NEXT,
        EV_REVIEW_PREV,
        EV_REVIEW_EXIT,
        EV_SENSOR_DETECTED,
        EV_SENSOR_REMOVED,
        EV_POWER_OFF,
        EV_POST_SUCCESS, // 220
        EV_POST_FAILED,
        EV_SAMPLE_DETECTED,
        EV_BATTERY_LEVEL,
        EV_BATTERY_CRITICAL,
        EV_LOW_POWER,
        EV_SLEEP_MODE,
        EV_START_CHARGE,
        EV_CHARGE_COMPLETE,
        EV_BTN_CFG_EXIT,
        EV_ACCEPT_LOT, // 230
        EV_REJECT_LOT,
        EV_BUTTON_DETECTED,
        EV_SAMPLE_TYPE_VENOUS,
        EV_SAMPLE_TYPE_CAPILLARY,
        EV_SAMPLE_TYPE_QC,
        EV_SAMPLE_QC_LEVEL_1,
        EV_SAMPLE_QC_LEVEL_2,
        EV_SAMPLE_QC_LEVEL_3,
        EV_OPER_ACK_ERROR,
        EV_OPER_ACK_SHUTDOWN, // 240
        EV_OPER_ACK_CANCEL_CHARGE,
        EV_EXIT_FAST_CHARGE,
        EV_EXIT_APP,
        EV_CANCEL_KEYBOARD,
        EV_KEYBOARD_DATA,
        EV_SCREEN_CREATED,
        EV_PREPARE_READY,
        EV_START_MEASURE,  // 50
        EV_MEASURE_COMPLETE,
        EV_MEASURED_RESULTS,
        EV_PATIENT_RESULT,
        EV_QC_RESULT,
        EV_CONFIG_DATA,
        EV_MEASURE_CANCELLED,
        EV_PID_UPDATE,
        EV_KEYBOARD_DATA_UPDATE,
        EV_TOUCH_EVENT,
        EV_UNLOCK,
        EV_BATTERY_STATUS,   // 256
        EV_WALL_POWER_OFF,   // 257
        EV_WALL_POWER_ON,    // 258
        EV_PID_CONFIRMED,
        EV_UI_READY, // 260
        EV_I2C_READY,
        EV_ETH_READY,
        EV_BT_READY,
        EV_USB_READY,
        EV_DAQ_READY,
        EV_HbDAQ_READY,
        EV_TOUCH_READY,
        EV_COMMS_READY,
        EV_TEMP_RANGE_HIGH,
        EV_TEMP_RANGE_LOW,   // 270
        EV_TEMP_RANGE_OK,
        EV_TEMP_RANGE_UNSTABLE,
        EV_TEMP_STATE_CHANGE,
        EV_CAL_BUTTON_PRESENT,
        EV_NO_CAL_BUTTON,
        EV_SENSOR_OK,
        EV_SUSPEND_TICK,
        EV_RESUME_TICK,
        EV_SENSOR_TIMEOUT,
        EV_BT_SCAN_REPORT,
        EV_BT_DISCONNECTED,  // 280
        EV_BT_CONNECTED,
        EV_BT_DATA,
        EV_BT_CONNECT_FAILED,
        EV_EXIT_BATTERY_CRITICAL,
        EV_SLEEP_TIMER,
        EV_BLUETOOTH_TIMER,
        EV_UI_PROXY_READY,
        EV_CONTROL_PROXY_READY,
        EV_ACTIVE_LOT_DATA,
        EV_DIAG_MODE_START,
        EV_DIAG_MODE_END,
        EV_TIMER_NEW_KIT_MSG,
        EV_TIMER_POST_TIMER,
        EV_EVENT_LOG,
        EV_QC_ACCEPTED,
        EV_QC_REJECTED,
        EV_SYSTEM_STATE_CHANGE,
        EV_CONTROL_STARTUP,
        EV_STARTUP_TIMER,
        EV_FSM_READY,
        EV_SIM_MEASURE_TIMER,
        LAST_EVENT,
    };

    public enum MessageCommandEnum
    {
        FIRST_COMMAND = 300,
        CMD_MEASURE_SAMPLE,
        CMD_CANCEL_MEASURE,
        CMD_CHECK_SENSOR,
        CMD_WAIT_FOR_SAMPLE,
        CMD_SHOW_INSERT,
        CMD_SHOW_POST,
        CMD_SHOW_POST_PASSED,
        CMD_SHOW_POST_ERROR,
        CMD_SHOW_PREPARE,
        CMD_SHOW_APPLY_SAMPLE,
        CMD_SHOW_QC_LEVEL,
        CMD_SHOW_MEASURE,
        CMD_SHOW_RESULTS,
        CMD_SHOW_REMOVE,
        CMD_SHOW_ERROR,
        CMD_SHOW_ERROR_MSG,
        CMD_SHOW_KEYBOARD,
        CMD_SHOW_CAL,
        CMD_SHOW_NOCAL,
        CMD_SHOW_REVIEW,
        CMD_SHOW_CONFIG,
        CMD_SHOW_FAST_CHARGE,
        CMD_SHOW_CHARGE_STATUS,
        CMD_SHOW_CHARGE_COMPLETE,
        CMD_SHOW_CHARGE_CANCELLED,
        CMD_SHOW_SHUTDOWN,
        CMD_SHOW_QC_DATA,
        CMD_SHOW_CANCEL_CHARGE,
        CMD_SHOW_CFG_PID,        // display PID required config screen
        CMD_SHOW_CFG_SECURITY,   // display security required config screen
        CMD_SHOW_CFG_TONES,   // display audible tones config screen
        CMD_SHOW_CFG_UNITS,   // display audible tones config screen
        CMD_SHOW_CFG_DATE,   // display audible tones config screen
        CMD_UPDATE_KEYBOARD_FIELD,
        CMD_SHOW_CONFIRM_PID,
        CMD_SHOW_NEW_BUTTON,
        CMD_SHOW_MENU,
        MSG_DEFAULT = 350,// do nothing
        MSG_UPDATE_DISPLAY,// redraw the display
        MSG_UPDATE_INPUT,// update the input
        MSG_TOUCH_EVENT,// touchscreen activity
        MSG_IP_CHANGE,// IP address change
        MSG_KB_EVENT,// Keyboard activity
        MSG_BC_EVENT,// Barcode activity
        CMD_SHOW_TEMP_HIGH,
        CMD_SHOW_TEMP_LOW,
        CMD_SHOW_TEMP_UNSTABLE,
        CMD_START_SLEEP_MODE,
        CMD_EXIT_SLEEP_MODE,
        CMD_GET_FIRST_UNSENT_PAT_SAMPLE,
        CMD_GET_NEXT_UNSENT_PAT_SAMPLE,
        CMD_GET_FIRST_UNSENT_QC_SAMPLE,
        CMD_GET_NEXT_UNSENT_QC_SAMPLE,
        CMD_FIRST_EVENT_ENTRY,
        CMD_NEXT_EVENT_ENTRY,
        CMD_GET_CONFIG,
        CMD_BT_CONNECT,
        CMD_BT_FORGET,
        CMD_BT_PAIR,
        CMD_BT_SCAN,
        CMD_BT_SLEEP,
        CMD_BT_WAKE,
        CMD_UPDATE_CONFIG,
        CMD_ACTIVATE_BUTTON,
        CMD_REJECT_BUTTON,
        CMD_SET_TEMP_HIGH,
        CMD_SET_TEMP_LOW,
        CMD_SET_TEMP_UNSTABLE,
        CMD_SET_TEMP_NORMAL,
        CMD_RUN_POST,
        MSG_BT_SCAN_REPORT,
        CMD_GET_EVENT_LOG,
        CMD_RESET_STORAGE,
        CMD_MARK_PAT_SAMPLE_SENT,
        CMD_MARK_QC_SAMPLE_SENT,
        CMD_GET_LAST_PAT_SAMPLE,
        CMD_GET_LAST_QC_SAMPLE,
        CMD_CONTROL_STARTUP,
        LAST_COMMAND,
    } ;

    public enum SystemStateEnum
    {
        // FSM States
        ST_FSM_NO_STATE = 32,     // 32
        ST_FSM_POST,            // 33 Power-On-Self-Test Running
        ST_FSM_READY,           // 34 Waiting for User to Insert Test Strip
        ST_FSM_NO_CAL,          // 35 The analyzer requires calibration
        ST_FSM_REMOTE,          // 36 The analzyer is downloading results to the host
        ST_FSM_MEASURE,         // 37 A test is running
        ST_FSM_PREPARE_SAMPLE,  // 38 Waiting for the user to apply the sample to the test strip
        ST_FSM_REMOVE,          // 39 Waiting for the user to remove the test strip.
        ST_FSM_RESULTS,         // 40
        ST_FSM_SAMPLE_REVIEW,   // 41
        ST_FSM_SHUTDOWN,        // 42
        ST_FSM_ERROR,           // 43
        ST_FSM_CONFIG,          // 44
        ST_FSM_REVIEW_SAMPLE,   // 45
        ST_FSM_NEW_BUTTON,      // 46
        ST_FSM_TEST_MODE,       // 47
        ST_FSM_LOCKED,          // 48
        ST_FSM_OPTIONS_STATE,   // 49
        ST_FSM_LAST_STATE,      // 50
    };

    public enum SystemTasksEnum
    {
        UI_TASK,
        FSM_TASK,
        TOUCH_TASK,
        USB_TASK,
        BT_TASK,
        COMMS_TASK,
        TCP_TASK,
        DAQ_TASK,
        HbDAQ_TASK,
        SUPERV_TASK,
        UI_PROXY_TASK,
        CONTROL_PROXY_TASK,
        HOST_TASK
    } ;


    public enum SampleSourceEnum : short
    {
        Capillary = 1,
        Venous,
        QCLevel1,
        QCLevel2,
        QCLevel3,
        HGBLevel1,
        HGBlevel2,
        HGBLevel3,
        Unknown,
    };

    public enum ResultTypeEnum : short
    {
        RESULT_PATIENT = 0,
        RESULT_QC
    }


    public enum UnitsEnum
    {
        None,
        ug_dL,
        g_dL,
        mm_L,
        per_est,
        mg_dL,
        ng_mL,
        umol_L,
        pmol_L,
    }
    public enum FlagsEnum
    {
        High,
        Low,
        None,
        Incalc
    }

    public class Constants
    {
        public static UInt16 LOT_LEN = 6;
        public static UInt16 PID_LEN = 24;
        public static UInt16 OID_LEN = 8;

        public static ushort SENT_BIT = 0x0001; // sample sent flag
        public static ushort CAPILLARY_BIT  = 0x0002; // Capillary Sample
        public static ushort VENOUS_BIT     = 0x0004; // Venous Sample

        public static ushort LEVEL_BIT_MASK =        0x0038; // Mask For All Level Bits
        public static ushort LEVEL_1_BITS =          0x0008; // Level 1 QC
        public static ushort LEVEL_2_BITS =          0x0010; // Level 2 QC
        public static ushort LEVEL_3_BITS =          0x0020; // Level 3 QC
        public static ushort LEVEL_4_BITS =          0x0018; // Level 4 QC HGB
        public static ushort LEVEL_5_BITS =          0x0028; // Level 5 QC HGB
        public static ushort LEVEL_6_BITS =          0x0038; // Level 6 QC HGB
    
        public static ushort SENSOR_ID_MASK =        0x0F00; // Sensor ID Mask
        public static ushort LEAD_SENSOR_ID =        0;
        public static ushort HCT_SENSOR_ID =         1;
        public static ushort BILIRUBIN_SENSOR_ID =   2;
        public static ushort O_TEST_SENSOR_ID =      3;
        public static ushort CHOLESTEROL_SENSOR_ID = 4;
        public static ushort SPARE5_SENSOR_ID =      5;
        public static ushort SPARE6_SENSOR_ID =      6;
        public static ushort SPARE7_SENSOR_ID =      7;
        public static ushort SPARE8_SENSOR_ID =      8;
        public static ushort SPARE9_SENSOR_ID =      9;
        public static ushort SPARE10_SENSOR_ID =     10;
        public static ushort SPARE11_SENSOR_ID =     11;
        public static ushort LEAD_HGB_SENSOR_ID =    12;
        public static ushort SPARE13_SENSOR_ID =     13;
        public static ushort SPARE14_SENSOR_ID =     14;
        public static ushort E_TEST_SENSOR_ID =      15;
        
        ///////////////////////////////////////////////////
        // Sample Result Bit Masks
        public static UInt16 G_DL_BIT       = 0x0001;
        public static UInt16 MMOL_L_BIT     = 0x0002;
        public static UInt16 UG_DL_BIT      = 0x0004;
        public static UInt16 NO_UNIT_BIT    = 0x0008;
        public static UInt16 NG_ML_BIT      = 0x0100;
        public static UInt16 PER_EST_BIT    = 0x1000;
        public static UInt16 MG_DL_BIT      = 0x2000;
        public static UInt16 UMOL_L_BIT     = 0x4000;
        public static UInt16 PMOL_L_BIT     = 0x8000;

        // Result Flags
        public static UInt16 RESULT_HIGH_BIT    = 0x0010;
        public static UInt16 RESULT_LOW_BIT     = 0x0020;
        public static UInt16 RESULT_INRANGE_BIT = 0x0040;
        public static UInt16 RESULT_INCALC_BIT  = 0x0080;
    }
}
