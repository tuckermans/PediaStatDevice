using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public enum ErrorCode: byte
    {
        ERR_NONE,
        ERR_REMOVE_SENSOR,     
        ERR_TEMP_RANGE,        
        ERR_TEMP_UNSTABLE_WAIT,
        ERR_SENSOR_LOT_OLD,    
        ERR_QC_CHECK_FAIL,     
        ERR_SENSOR_WET,        
        ERR_OUT_OF_VIAL,       
        ERR_LOW_BATTERY,       
        ERR_WETTING,       
        ERR_SENSOR_REMOVED,    
        ERR_DEP_TEMP_CHANGE,     
        ERR_RECAL,
        ERR_CAL,
        ERR_ASSAY_TEMP_RANGE,
        ERR_ASSAY_TEMP_CHANGE,
        ERR_MUTEX_FAIL,
        ERR_HIGH_CUR,
        ERR_UNSECURED_BUTTON
    }
}
