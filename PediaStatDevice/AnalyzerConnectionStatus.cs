using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class AnalyzerConnectionStatus : AnlayzerStatusEvent
    {
        public string SerialNumber
        {
            get;
            private set;
        }

        private AnalyzerConnectionStateEnum _ConnectionState = AnalyzerConnectionStateEnum.Disconnected;

        public AnalyzerConnectionStateEnum ConnectionStatus
        {
            get
            {
                return _ConnectionState;
            }
        }

        public AnalyzerConnectionStatus(AnalyzerConnectionStateEnum state, string serial=null)
        {
            _ConnectionState = state;
            SerialNumber = serial;
        }
    };
}
