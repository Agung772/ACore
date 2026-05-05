using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ACore
{
    public static class Encryption
    {
        private const string KEY = "abcdefghijklmnop";
        
        public static byte[] Encrypt(string value)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(KEY);
                Array.Resize(ref keyBytes, aes.KeySize / 8);
    
                aes.Key = keyBytes;
                aes.GenerateIV();
    
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(value);
                    }
                    return ms.ToArray();
                }
            }
        }
        public static string Decrypt(byte[] encrypt)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(KEY);
                Array.Resize(ref keyBytes, aes.KeySize / 8);
    
                aes.Key = keyBytes;
    
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(encrypt, iv, iv.Length);
    
                using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                using (var ms = new MemoryStream(encrypt, iv.Length, encrypt.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}

