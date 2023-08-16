using System;
using System.Security.Cryptography;
using System.Text;

namespace Fanex.NAP.SPU.DCS.JobService.Ultilities
{
    public static class Cipher
    {
        public static Int64 Hashing2Int(string original)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(original));

                return BitConverter.ToInt64(bytes, 0);
            }
        }
    }
}

