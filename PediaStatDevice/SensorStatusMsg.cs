using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class SensorStatusMsg
    {
        public bool _state = false;
        public SensorIDEnum _id;

        public SensorStatusMsg(byte[] payload)
        {           
            byte status = payload[0];

            _state = (status == 1) ? true : false;
            if (_state == true)
            {
                _id = (SensorIDEnum)payload[1];
            }       
        }
    }
}
