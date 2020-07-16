using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public enum AnalyzerConnectionStateEnum
    {
        Connected,
        Disconnected
    }

    public class AnlayzerStatusEvent : EventArgs
    {
        public string Serial
        {
            get;
            set;
        }
    }

  

    public class AnalyzerState : AnlayzerStatusEvent
    {
        private SystemStateEnum _state = SystemStateEnum.ST_FSM_NO_STATE;

        public SystemStateEnum State
        {
            get
            {
                return _state;
            }
        }

        public AnalyzerState(SystemStateEnum state)
        {
            _state = state;
        }
    }

}
