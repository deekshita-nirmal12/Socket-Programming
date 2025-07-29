using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;

public class EncryptionHelper
{
	private static readonly byte[] Key = Encoding.UTF8.GetBytes("ThisIsASecretKey"); 
	private static readonly byte[] IV = Encoding.UTF8.GetBytes("ThisIsAnIV456789"); 

	public static string Encrypt(string plainText)
	{
		using Aes aes = Aes.Create();
		aes.Key = Key;
		aes.IV = IV;
		using MemoryStream ms = new();
		using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
		using StreamWriter sw = new(cs);
		sw.Write(plainText);
		sw.Flush();
		cs.FlushFinalBlock();
		return Convert.ToBase64String(ms.ToArray());
	}

	public static string Decrypt(string encryptedText)
	{
		byte[] buffer = Convert.FromBase64String(encryptedText);
		using Aes aes = Aes.Create();
		aes.Key = Key;
		aes.IV = IV;
		using MemoryStream ms = new(buffer);
		using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
		using StreamReader sr = new(cs);
		return sr.ReadToEnd();
	}
}
