
namespace PediaStatDevice
{
    public enum PortStatusEnum: byte
    {
        Empty,
        TestStripIn,
        QCStripNoTest,
        QCStripIn,
        Unknown
    }
    public class PortStatusMessage
    {
        public bool SensorDetected { get; private set; }

        public PortStatusEnum PortStatus { get; private set; }
        public PortStatusMessage(byte data)
        {
            SensorDetected = (data != 0);
            PortStatus = PortStatusEnum.Unknown;
            if (data < (byte)PortStatusEnum.Unknown)
            {
                PortStatus = (PortStatusEnum)data;
            }
        }
    }
}
