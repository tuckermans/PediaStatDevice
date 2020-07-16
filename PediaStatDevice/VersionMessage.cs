using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class VersionMessage
    {
        public string Version { get; private set; }

        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Revision { get; private set; }

        public float FloatVersion { get; private set; }

        public VersionMessage(byte[] data)
        {
            Int16 v = BitConverter.ToInt16(data, 0);
            Major = (v / 100);
            Minor = ((v % 100) / 10);
            Revision = (v % 10);

            Version = string.Format("{0}.{1}{2}", Major, Minor, Revision);
            FloatVersion = (float)v / 100.0f;
        }
    }
}
