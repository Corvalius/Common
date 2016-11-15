using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace System.Security
{
    /// <summary>
    /// Provides extension methods that deal with
    /// string encryption/decryption and
    /// <see cref="SecureString"/> encapsulation.
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Specifies the data protection scope of the DPAPI.
        /// </summary>
        private const DataProtectionScope Scope = DataProtectionScope.CurrentUser;

        /// <summary>
        /// Encrypts a given password and returns the encrypted data
        /// as a base64 string.
        /// </summary>
        /// <param name="plainText">An unencrypted string that needs
        /// to be secured.</param>
        /// <returns>A base64 encoded string that represents the encrypted
        /// binary data.
        /// </returns>
        /// <remarks>This solution is not really secure as we are
        /// keeping strings in memory. If runtime protection is essential,
        /// <see cref="SecureString"/> should be used.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="plainText"/>
        /// is a null reference.</exception>
        public static string Encrypt(this string plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            //encrypt data
            byte[] data = Encoding.Unicode.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, Scope);

            //return as base64 string
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="cipher">A base64 encoded string that was created
        /// through the <see cref="Encrypt(string)"/> or
        /// <see cref="Encrypt(SecureString)"/> extension methods.</param>
        /// <returns>The decrypted string.</returns>
        /// <remarks>Keep in mind that the decrypted string remains in memory
        /// and makes your application vulnerable per se. If runtime protection
        /// is essential, <see cref="SecureString"/> should be used.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="cipher"/>
        /// is a null reference.</exception>
        public static string Decrypt(this string cipher)
        {
            if (cipher == null) throw new ArgumentNullException("cipher");

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);

            //decrypt data
            byte[] decrypted = ProtectedData.Unprotect(data, null, Scope);
            return Encoding.Unicode.GetString(decrypted);
        }

        /// <summary>
        /// Encrypts the contents of a secure string.
        /// </summary>
        /// <param name="value">An unencrypted string that needs
        /// to be secured.</param>
        /// <returns>A base64 encoded string that represents the encrypted
        /// binary data.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/>
        /// is a null reference.</exception>
        public static string Encrypt(this SecureString value)
        {
            if (value == null) throw new ArgumentNullException("value");

            IntPtr ptr = Marshal.SecureStringToCoTaskMemUnicode(value);
            try
            {
                var buffer = new char[value.Length];
                Marshal.Copy(ptr, buffer, 0, value.Length);

                byte[] data = Encoding.Unicode.GetBytes(buffer);
                byte[] encrypted = ProtectedData.Protect(data, null, Scope);

                //return as base64 string
                return Convert.ToBase64String(encrypted);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptr);
            }
        }

        /// <summary>
        /// Decrypts a base64 encrypted string and returns the decrpyted data
        /// wrapped into a <see cref="SecureString"/> instance.
        /// </summary>
        /// <param name="cipher">A base64 encoded string that was created
        /// through the <see cref="Encrypt(string)"/> or
        /// <see cref="Encrypt(SecureString)"/> extension methods.</param>
        /// <returns>The decrypted string, wrapped into a
        /// <see cref="SecureString"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="cipher"/>
        /// is a null reference.</exception>
        public static SecureString DecryptSecure(this string cipher)
        {
            if (cipher == null) throw new ArgumentNullException("cipher");

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);

            //decrypt data
            byte[] decrypted = ProtectedData.Unprotect(data, null, Scope);
            char[] decryptedChars = Encoding.Unicode.GetChars(decrypted);

            var ss = new SecureString();
            for (int i = 0; i < decryptedChars.Length; i++)
            {
                ss.AppendChar(decryptedChars[i]);

                // We overwrite the memory storing the password in memory, just in case.
                decrypted[i] = 0;
                decryptedChars[i] = '0';
            }

            //mark as read-only
            ss.MakeReadOnly();
            return ss;
        }

        /// <summary>
        /// Wraps a managed string into a <see cref="SecureString"/> 
        /// instance.
        /// </summary>
        /// <param name="value">A string or char sequence that 
        /// should be encapsulated.</param>
        /// <returns>A <see cref="SecureString"/> that encapsulates the
        /// submitted value.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/>
        /// is a null reference.</exception>
        public static SecureString ToSecureString(this IEnumerable<char> value)
        {
            if (value == null) throw new ArgumentNullException("value");

            var secured = new SecureString();

            char[] charArray = value.ToArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                secured.AppendChar(charArray[i]);
            }

            secured.MakeReadOnly();
            return secured;
        }

        /// <summary>
        /// Unwraps the contents of a secured string and
        /// returns the contained value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>Be aware that the unwrapped managed string can be
        /// extracted from memory.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/>
        /// is a null reference.</exception>
        public static string Unwrap(this SecureString value)
        {
            if (value == null) throw new ArgumentNullException("value");

            IntPtr ptr = Marshal.SecureStringToCoTaskMemUnicode(value);
            try
            {
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptr);
            }
        }

        /// <summary>
        /// Checks whether a <see cref="SecureString"/> is either
        /// null or has a <see cref="SecureString.Length"/> of 0.
        /// </summary>
        /// <param name="value">The secure string to be inspected.</param>
        /// <returns>True if the string is either null or empty.</returns>
        public static bool IsNullOrEmpty(this SecureString value)
        {
            return value == null || value.Length == 0;
        }

        /// <summary>
        /// Performs bytewise comparison of two secure strings.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <returns>True if the strings are equal.</returns>
        public static bool Matches(this SecureString value, SecureString other)
        {
            if (value == null && other == null) return true;
            if (value == null || other == null) return false;
            if (value.Length != other.Length) return false;
            if (value.Length == 0 && other.Length == 0) return true;

            IntPtr ptrA = Marshal.SecureStringToCoTaskMemUnicode(value);
            IntPtr ptrB = Marshal.SecureStringToCoTaskMemUnicode(other);
            try
            {
                //parse characters one by one - doesn't change the fact that
                //we have them in memory however...
                byte byteA = 1;
                byte byteB = 1;

                int index = 0;
                while (((char)byteA) != '\0' && ((char)byteB) != '\0')
                {
                    byteA = Marshal.ReadByte(ptrA, index);
                    byteB = Marshal.ReadByte(ptrB, index);
                    if (byteA != byteB) return false;
                    index += 2;
                }

                return true;
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptrA);
                Marshal.ZeroFreeCoTaskMemUnicode(ptrB);
            }
        }


        public static string EncryptTripleDES(this string input)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            
            var tripleDES = new TripleDESCryptoServiceProvider 
            {
                Key = new byte[] { 176, 211, 93, 72, 123, 5, 68, 196, 84, 141, 129, 138, 225, 191, 180, 185, 88, 219, 69, 99, 0, 169, 69, 32 },
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7 
            };

            var cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string DecryptTripleDES(this string input)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            
            var tripleDES = new TripleDESCryptoServiceProvider 
            {
                Key = new byte[] { 176, 211, 93, 72, 123, 5, 68, 196, 84, 141, 129, 138, 225, 191, 180, 185, 88, 219, 69, 99, 0, 169, 69, 32 },
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
