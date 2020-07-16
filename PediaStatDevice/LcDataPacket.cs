using System;
using System.IO;

namespace PediaStatDevice
{
    public class LcDataPacket
    {
        public CmdIDType Command { get; private set; }
        public ushort Length { get; set; }
        public byte[] Data { get; set; }
        public ushort CRC { get; set; }

        public LcDataPacket(CmdIDType cmd)
        {
            Command = cmd;
        }

        public LcDataPacket(CmdIDType cmd, byte[] data)
            : this(cmd)
        {
            if (data != null)
            {
                SetData(data);
            }
        }

        /// <summary>
        /// Set the packet data buffer.
        /// Update payload length
        /// </summary>
        /// <param name="data"></param>
        public void SetData(byte[] data)
        {
            if (Data != null)
            {
                Data = null;
            }
            Data = new byte[data.Length];
            Array.Copy(data, Data, data.Length);
            Length = (ushort)data.Length;
        }

        /// <summary>
        /// Build an command packet based on the ID and payload data set on the instance.
        /// </summary>
        /// <returns></returns>
        public byte[] BuildPacket(Crc16 crcMaker)
        {
            MemoryStream strm = new MemoryStream();
            strm.WriteByte((byte)ControlCode.SOH);
            strm.WriteByte((byte)Command);
            ushort len = 0;
            if (Data != null)
            {
                len = (ushort)Data.Length;
                strm.Write(BitConverter.GetBytes(len), 0, 2);
                strm.Write(Data, 0, len);
            }
            else
            {
                strm.Write(BitConverter.GetBytes(len), 0, 2);
            }
            byte[] packet = strm.ToArray();
            ushort crc = crcMaker.ComputeChecksum(packet, 1, packet.Length - 1);
            strm.Write(BitConverter.GetBytes(crc), 0, 2);

            packet = strm.ToArray();

            strm.Close();

            return packet;
        }
        public byte[] BuildPacketLut(Crc16Lut crcMaker)
        {
            MemoryStream strm = new MemoryStream();
            strm.WriteByte((byte)ControlCode.SOH);
            strm.WriteByte((byte)Command);
            ushort len = 0;
            if (Data != null)
            {
                len = (ushort)Data.Length;
                strm.Write(BitConverter.GetBytes(len), 0, 2);
                strm.Write(Data, 0, len);
            }
            else
            {
                strm.Write(BitConverter.GetBytes(len), 0, 2);
            }
            byte[] packet = strm.ToArray();
            ushort crc = crcMaker.ComputeChecksum(packet, 1, packet.Length - 1);
            strm.Write(BitConverter.GetBytes(crc), 0, 2);

            packet = strm.ToArray();

            strm.Close();

            return packet;
        }
    }
}
