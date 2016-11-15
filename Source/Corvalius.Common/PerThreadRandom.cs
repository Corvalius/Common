using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Corvalius
{
    public class PerThreadRandom
    {
        private static readonly RNGCryptoServiceProvider Global = new RNGCryptoServiceProvider();

        [ThreadStatic]
        private static Random local;

        public static int Next(int max = Int32.MaxValue)
        {
            Random instance = local;
            if (instance == null)
            {
                var buffer = new byte[4];
                Global.GetBytes(buffer);
                local = instance = new Random(BitConverter.ToInt32(buffer, 0));
            }

            return instance.Next() % max;
        }

        private const string AlphanumericCharacters = "abcdefghijklmnopqrstuvwxyz1234567890";
        private const string NumericCharacters = "1234567890";

        public static string NextString(int size, bool onlyNumbers = false)
        {
            var buffer = new char[size];

            string characterSet = onlyNumbers ? NumericCharacters : AlphanumericCharacters;

            for (int i = 0; i < size; i++)
            {
                buffer[i] = characterSet[PerThreadRandom.Next(characterSet.Length)];
            }
            return new string(buffer);
        }
    }
}
