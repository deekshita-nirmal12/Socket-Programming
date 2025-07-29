using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
	static Dictionary<string, Dictionary<string, int>> mainDict = new(StringComparer.OrdinalIgnoreCase)
	{
		{ "SetA", new(StringComparer.OrdinalIgnoreCase) { { "One", 1 }, { "Two", 2 } } },
		{ "SetB", new(StringComparer.OrdinalIgnoreCase) { { "Three", 3 }, { "Four", 4 } } },
		{ "SetC", new(StringComparer.OrdinalIgnoreCase) { { "Five", 5 }, { "Six", 6 } } },
		{ "SetD", new(StringComparer.OrdinalIgnoreCase) { { "Seven", 7 }, { "Eight", 8 } } },
		{ "SetE", new(StringComparer.OrdinalIgnoreCase) { { "Nine", 9 }, { "Ten", 10 } } }
	};

	static void Main()
	{
		try
		{
			Console.Write("Enter port to listen on: ");
			if (!int.TryParse(Console.ReadLine(), out int port))
			{
				Console.WriteLine("Invalid port number. Exiting.");
				return;
			}

			TcpListener listener = new(IPAddress.Any, port);
			listener.Start();
			Console.WriteLine($"Server started on port {port}. Waiting for connections...");

			while (true)
			{
				TcpClient client = listener.AcceptTcpClient();
				new Thread(() => HandleClient(client)).Start();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error starting server: {ex.Message}");
		}
	}

	static void HandleClient(TcpClient client)
	{
		try
		{
			using NetworkStream stream = client.GetStream();
			while (true)
			{
				string input;
				try
				{
					input = ReceiveMessage(stream);
				}
				catch
				{
					Console.WriteLine("Client disconnected unexpectedly.");
					break;
				}

				string decryptedInput = EncryptionHelper.Decrypt(input);
				Console.WriteLine($"Received from client (decrypted): {decryptedInput}");

				if (decryptedInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine("Client requested to exit.");
					break;
				}

				string[] parts = decryptedInput.Split('-', 2);
				if (parts.Length != 2)
				{
					SendMessage(stream, "Invalid Input Format. Use format: SetX-Word");
					SendMessage(stream, "END");
					continue;
				}

				string mainKey = parts[0].Trim();
				string subKey = parts[1].Trim();

				if (!mainDict.TryGetValue(mainKey, out var subDict) || !subDict.TryGetValue(subKey, out int value))
				{
					SendMessage(stream, "Invalid Set or Word");
					SendMessage(stream, "END");
					continue;
				}

				// Send current time at 1-second interval 'value' times
				for (int i = 0; i < value; i++)
				{
					string msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					SendMessage(stream, msg);
					Thread.Sleep(1000);
				}

				SendMessage(stream, "END");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}
		finally
		{
			client.Close();
		}
	}

	static void SendMessage(NetworkStream stream, string plainText)
	{
		string encrypted = EncryptionHelper.Encrypt(plainText);
		byte[] data = Encoding.UTF8.GetBytes(encrypted);
		byte[] length = BitConverter.GetBytes(data.Length);
		stream.Write(length, 0, 4);
		stream.Write(data, 0, data.Length);
	}

	static string ReceiveMessage(NetworkStream stream)
	{
		byte[] lengthBuffer = ReadExactBytes(stream, 4);
		int length = BitConverter.ToInt32(lengthBuffer, 0);
		byte[] buffer = ReadExactBytes(stream, length);
		return Encoding.UTF8.GetString(buffer);
	}

	static byte[] ReadExactBytes(NetworkStream stream, int size)
	{
		byte[] buffer = new byte[size];
		int totalRead = 0;
		while (totalRead < size)
		{
			int read = stream.Read(buffer, totalRead, size - totalRead);
			if (read == 0) throw new IOException("Disconnected while reading data");
			totalRead += read;
		}
		return buffer;
	}
}
