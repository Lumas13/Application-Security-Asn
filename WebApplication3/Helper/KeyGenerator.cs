using System;
using System.Security.Cryptography;
using System.Text;

public class KeyGenerator
{
	public static string GenerateRandomKey(int keySize)
	{
		using (var rng = new RNGCryptoServiceProvider())
		{
			byte[] keyBytes = new byte[keySize / 8]; // Key size in bits divided by 8 to get byte size
			rng.GetBytes(keyBytes);
			return Convert.ToBase64String(keyBytes);
		}
	}

	public static string GenerateRandomIV(int blockSize)
	{
		using (var rng = new RNGCryptoServiceProvider())
		{
			byte[] ivBytes = new byte[blockSize / 8]; // Block size in bits divided by 8 to get byte size
			rng.GetBytes(ivBytes);
			return Convert.ToBase64String(ivBytes);
		}
	}

}
