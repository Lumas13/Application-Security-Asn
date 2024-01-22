using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESEncryptionHelper
{
    public static string Encrypt(string plainText, string key, string iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(key);
            aesAlg.IV = Convert.FromBase64String(iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
                    csEncrypt.Write(plaintextBytes, 0, plaintextBytes.Length);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
}

