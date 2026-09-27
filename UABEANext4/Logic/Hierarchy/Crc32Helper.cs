using System.Text;

namespace UABEANext4.Logic.Hierarchy
{
    /// <summary>
    /// CRC-32 (IEEE 802.3) utility matching Unity's animation binding hash algorithm.
    /// Unity uses this to hash hierarchical object paths for AnimationClip bindings.
    /// </summary>
    public static class Crc32Helper
    {
        private static readonly uint[] CrcTable;

        static Crc32Helper()
        {
            CrcTable = new uint[256];
            const uint polynomial = 0xEDB88320u; // IEEE 802.3 polynomial (reversed)

            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
                CrcTable[i] = crc;
            }
        }

        /// <summary>
        /// Compute CRC-32 (IEEE 802.3) of a UTF-8 string, matching Unity's algorithm.
        /// </summary>
        public static uint ComputeCrc32(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0;

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            uint crc = 0xFFFFFFFFu;

            foreach (byte b in bytes)
            {
                crc = (crc >> 8) ^ CrcTable[(crc ^ b) & 0xFF];
            }

            return crc ^ 0xFFFFFFFFu;
        }
    }
}
