using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace PediaStatDevice
{
    public class StringUtils
    {

        /// <summary>
        /// Helper method to properly decode an ASCII string in a byte sequence
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns>string representation of the specified byte sequence</returns>
        internal static string DecodeASCIIBytes(byte[] rawData, int offset, int len)
        {
            string decoded = string.Empty;
            int nullIdx = offset;
            bool nullFound = false;
            for (int idx = 0; idx < len && nullFound == false; idx++)
            {
                if (rawData[idx + offset] == (byte)0)
                {
                    nullFound = true;
                }
                else
                {
                    nullIdx++;
                }
            }

            if (nullIdx != offset)
            {

                if (nullIdx < offset + len)
                {
                    decoded = Encoding.ASCII.GetString(rawData, offset, nullIdx-offset);
                }
                else
                {
                    decoded = Encoding.ASCII.GetString(rawData, offset, len);
                }
            }
            return decoded;
        }

        static private byte[] Key           
        {
            get
            {
                return Encoding.ASCII.GetBytes("ul8x1Ds8CZyWLBqW2jam");
            }
        }

        public static byte[] encrypt(String patientID, int length)
        {
            int i;
          
            byte[] obfuscatedID = new byte[length];

            for (i = 0; i < length; i++)
            {
                if (patientID[i] == '\0')
                    break;

                obfuscatedID[i] = (byte)(((( patientID[i] - 32) + ( Key[i] - 32) ) % 96) + 32);
            }
            return obfuscatedID;
        }

        public static string decrypt(byte[] patientID, int length)
        {
            int i;

            char[] obfuscatedID = new char[length];

            for (i = 0; i < length; i++)
            {

                char p1 = (char)(patientID[i] - 32);

                char p2 = (char)( ((int)(96 - Key[i])) + 32);

                obfuscatedID[i] = (char)( ((p1 + p2) % 96) +32);

            }
            return new String(obfuscatedID);
        }

        //***********************************
        // PASSWORD OF THE HOUR COMPUTATIONS
        //***********************************
        //constants used in SHA computation
        static private ulong[] KTN = { 0x5a827999L, 0x6ed9eba1L, 0x8f1bbcdcL, 0xca62c1d6L };

        static private ulong[] MTword = new ulong[80];      // 80 Elements
        private byte[] mac_input = new byte[64];     // 64 Elements
        private byte[] calc_mac = new byte[20];      // 20 Elements

        static private byte[] ComputeSHA(string MT)
        {
            ulong[] hash = new ulong[5];
            int j, i, offset;
            ulong ShftTmp;
            ulong Temp;
            byte[] mac_data = new byte[PSWD_KEY_SIZE];

            for (i = 0; i < 16; i++)
            {
                MTword[i] = (((ulong)MT[i * 4] & 0x00FF) << 24)
                        | (((ulong)MT[i * 4 + 1] & 0x00FF) << 16)
                        | (((ulong)MT[i * 4 + 2] & 0x00FF) << 8)
                        | ((ulong)MT[i * 4 + 3] & 0x00FF);
            }

            for (; i < 80; i++)
            {
                ShftTmp = (MTword[i - 3] ^ MTword[i - 8] ^ MTword[i - 14] ^ MTword[i - 16]);
                MTword[i] = (((ShftTmp << 1) & 0xFFFFFFFEL) | ((ShftTmp >> 31) & 0x00000001L));
            }

            hash[0] = 0x67452301L;
            hash[1] = 0xEFCDAB89L;
            hash[2] = 0x98BADCFEL;
            hash[3] = 0x10325476L;
            hash[4] = 0xC3D2E1F0L;

            for (i = 0; i < 80; i++)
            {
                ShftTmp = ((hash[0] << 5) & 0xFFFFFFE0L) | ((hash[0] >> 27) & 0x0000001F);
                Temp = NLF(hash[1], hash[2], hash[3], i) + hash[4] + KTN[i / 20] + MTword[i] + ShftTmp;
                hash[4] = hash[3];
                hash[3] = hash[2];
                hash[2] = ((hash[1] << 30) & 0xC0000000L) | ((hash[1] >> 2) & 0x3FFFFFFFL);
                hash[1] = hash[0];
                hash[0] = Temp;
            }

            // Clean up the MTword buffer
            for (int k = 0; k < 80; k++)
                MTword[k] = 0xFF;

            //iButtons use LSB first, so we have to turn
            //the result around a little bit.  Instead of
            //result A-B-C-D-E, our result is E-D-C-B-A,
            //where each letter represents four bytes of
            //the result.
            for (j = 4; j >= 0; j--)
            {
                Temp = hash[j];
                offset = (4 - j) * 4;
                for (i = 0; i < 4; i++)
                {
                    mac_data[i + offset] = (byte)Temp;
                    Temp >>= 8;
                }
            }

            return (mac_data);
        }

        // calculation used for the SHA MAC
        static private ulong NLF(ulong B, ulong C, ulong D, int n)
        {
            if (n < 20)
                return ((B & C) | ((~B) & D));
            else if (n < 40)
                return (B ^ C ^ D);
            else if (n < 60)
                return ((B & C) | (B & D) | (C & D));
            else
                return (B ^ C ^ D);
        }

        private const string DecodeString = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789";
        private const int NUM_PSWD_BITS = 35;
        private const int PSWD_KEY_SIZE = 64;

        private byte[] input = new byte[PSWD_KEY_SIZE];

        static public String PasswordAlgo(DateTime timestamp, string serNum)
        {
            UInt64 x;
            UInt64 selector;
            UInt64 numValue = 0;
            int baseN;
            int i;
            int index = 0;
            int digits;
            byte[] pswd;

            string input = string.Format("Y3JFOID3QTW{0:D2}AIA67{1:D2}Z07{2:D2}6X5XABIKRLG1TPQ8Y{3:D5}50A6AE8OLAJUX{4:D2}N2",
                                          Math.Abs(timestamp.Year - 2013), timestamp.Month, timestamp.Day, Convert.ToInt32(serNum.Substring(2, 5)), timestamp.Hour);

            pswd = ComputeSHA(input);

            // Create A Numerical Representation
            numValue = ((UInt64)pswd[0] << 28) | ((UInt64)pswd[1] << 20) | (((UInt64)pswd[9] & 0x0F) << 16) | ((UInt64)pswd[18] << 8) | (UInt64)pswd[19];
            numValue /= 2;

            // Convert Number To A Password
            baseN = DecodeString.Length;
            x = (UInt64)Math.Pow(2, NUM_PSWD_BITS);
            digits = 1;

            while ((x / (UInt64)Math.Pow(baseN, digits)) > (UInt64)baseN)
                digits++;

            Array.Clear(pswd, 0, PSWD_KEY_SIZE);

            for (i = digits; i >= 0; i--)
            {
                selector = (numValue / (UInt64)Math.Pow(baseN, i));
                numValue = numValue - (selector * (UInt64)Math.Pow(baseN, i));
                pswd[index++] = Convert.ToByte(DecodeString[(int)selector]);
            }

            return Encoding.ASCII.GetString(pswd);
        }
    }
}
