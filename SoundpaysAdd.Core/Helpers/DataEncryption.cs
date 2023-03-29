using System.Security.Cryptography;
using System.Text;


namespace SoundpaysAdd.Core.Helpers
{
    public static class DataEncryption
    {
        public static string EncryptSHA512(string str)
        {
            byte[] hash;
            byte[] data = Encoding.ASCII.GetBytes(str);
            using (SHA512 sha512 = SHA512Managed.Create())
            {
                hash = sha512.ComputeHash(data);
            };

            // Converting the Byte Hash Into String
            StringBuilder result = new StringBuilder();

            // Declaring and initializing format 
            string format = "X2";

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString(format));
            }
            return result.ToString();
        }

        /// <summary>
        /// Encode String into Base64
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Base64 encoded string</returns>
        public static string EncodeBase64(string key)
        {
            byte[] data = Encoding.ASCII.GetBytes(key);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// convert base64 from bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ConvertBase64FromBytes(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        /// <summary>
        /// Generate Random String
        /// </summary>
        /// <returns>Random  String </returns>
        public static string GetUniqueKey()
        {
            int size = 22;
            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;
                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        /// <summary>
        /// used for masking input number like account number or credit card number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String MaskedNumber(string input)
        {
            StringBuilder sb = new StringBuilder(input);

            const int skipLeft = 4;
            const int skipRight = 4;

            int left = -1;

            for (int i = 0, c = 0; i < sb.Length; ++i)
            {
                if (Char.IsDigit(sb[i]))
                {
                    c += 1;

                    if (c > skipLeft)
                    {
                        left = i;

                        break;
                    }
                }
            }

            for (int i = sb.Length - 1, c = 0; i >= left; --i)
                if (Char.IsDigit(sb[i]))
                {
                    c += 1;

                    if (c > skipRight)
                        sb[i] = '*';
                }

            return sb.ToString();
        }
    }
}
