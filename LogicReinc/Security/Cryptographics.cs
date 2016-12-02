using LogicReinc.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Security
{
    public static class Cryptographics
    {

        public static byte[] Hash(byte[] bytes, HashType type)
        {
            switch (type)
            {
                case HashType.MD5:
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    bytes = md5.ComputeHash(bytes);
                    break;
                case HashType.Sha1:
                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    bytes = sha1.ComputeHash(bytes);
                    break;
                case HashType.Sha256:
                    SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                    bytes = sha256.ComputeHash(bytes);
                    break;
                case HashType.Sha512:
                    SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();
                    bytes = sha512.ComputeHash(bytes);
                    break;
            }

            return bytes;
        }
        public static string Hash(string toHash, HashType type)
        {
            return Convert.ToBase64String(Hash(Encoding.UTF8.GetBytes(toHash), type));
        }

        public static string SecureHash(string toHash, byte[] salt, int itterations, int length)
        {
            return Convert.ToBase64String(SecureHash(toHash.GetBytes(), salt, itterations, length));
        }

        public static byte[] SecureHash(byte[] toHash, byte[] salt, int itterations, int length)
        {
            using (Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes(toHash, salt, itterations))
                return derived.GetBytes(length);
        }

        public static byte[] GetSalt(int length)
        {
            var bytes = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            return bytes;
        }

        public static string Encrypt(string text, string password, string salting)
        {
            string encryptedString = string.Empty;
            RijndaelManaged algo = new RijndaelManaged();

            algo.Padding = PaddingMode.PKCS7;
            try
            {
                byte[] salt = Encoding.ASCII.GetBytes(salting);

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
                algo.Key = key.GetBytes(algo.KeySize / 8);
                algo.IV = key.GetBytes(algo.BlockSize / 8);

                ICryptoTransform encryptor = algo.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream encryptStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter sw = new StreamWriter(encryptStream))
                            sw.Write(text);
                    encryptedString = Convert.ToBase64String(ms.ToArray());
                }
            }
            finally
            {
                if (algo != null)
                    algo.Clear();
            }
            return encryptedString;
        }

        public static string Decrypt(string encryptedText, string password, string salting)
        {
            string plainString = string.Empty;
            RijndaelManaged algo = new RijndaelManaged();
            algo.Padding = PaddingMode.PKCS7;
            try
            {
                byte[] salt = Encoding.ASCII.GetBytes(salting);

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
                algo.Key = key.GetBytes(algo.KeySize / 8);
                algo.IV = key.GetBytes(algo.BlockSize / 8);

                ICryptoTransform decryptor = algo.CreateDecryptor();

                byte[] bytes = Convert.FromBase64String(encryptedText);
                using (MemoryStream ms = new MemoryStream(bytes))
                    using (CryptoStream decryptStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        using (StreamReader sr = new StreamReader(decryptStream))
                            plainString = sr.ReadToEnd();
            }
            finally
            {
                if (algo != null)
                    algo.Clear();
            }
            return plainString;
        }


        public static CryptoStream CreateEncryptStream(Stream str, string password, string saltStr)
        {
            RijndaelManaged algo = new RijndaelManaged();

            algo.Padding = PaddingMode.PKCS7;
            try
            {
                byte[] salt = Encoding.ASCII.GetBytes(saltStr);

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
                algo.Key = key.GetBytes(algo.KeySize / 8);
                algo.IV = key.GetBytes(algo.BlockSize / 8);

                ICryptoTransform encryptor = algo.CreateEncryptor();
                return new CryptoStream(str, encryptor, CryptoStreamMode.Write);
            }
            finally
            {
                if (algo != null)
                    algo.Clear();
            }
        }

        public static CryptoStream CreateDecryptStream(Stream str, string password, string saltStr)
        {
            RijndaelManaged algo = new RijndaelManaged();
            algo.Padding = PaddingMode.PKCS7;
            try
            {
                byte[] salt = Encoding.ASCII.GetBytes(saltStr);

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
                algo.Key = key.GetBytes(algo.KeySize / 8);
                algo.IV = key.GetBytes(algo.BlockSize / 8);

                ICryptoTransform decryptor = algo.CreateDecryptor();


                return new CryptoStream(str, decryptor, CryptoStreamMode.Read);
                
            }
            finally
            {
                if (algo != null)
                    algo.Clear();
            }
        }
    }

    public enum HashType
    {
        MD5,
        Sha1,
        Sha256,
        Sha512
    }
}
