using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class Client
{
	static void Main()
	{
		Console.OutputEncoding = Encoding.UTF8;

		while (true)
		{
			Console.Write("Enter server IP (or type 'exit' to quit): ");
			Console.Out.Flush();
			string ip = Console.ReadLine()?.Trim();
			if (string.Equals(ip, "exit", StringComparison.OrdinalIgnoreCase))
				break;

			Console.Write("Enter server port: ");
			if (!int.TryParse(Console.ReadLine(), out int port))
			{
				Console.WriteLine("Invalid port number. Please try again.\n");
				continue;
			}

			try
			{
				using TcpClient client = new(ip, port);
				using NetworkStream stream = client.GetStream();

				Console.WriteLine("Connected to server.\n");

				while (true)
				{
					Console.Write("\nEnter string (e.g. SetA-Two or 'exit' to quit): ");
					string input = Console.ReadLine();

					if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
						return;

					string encrypted = EncryptionHelper.Encrypt(input);
					byte[] data = Encoding.UTF8.GetBytes(encrypted);
					byte[] length = BitConverter.GetBytes(data.Length);
					stream.Write(length, 0, length.Length);
					stream.Write(data, 0, data.Length);

					while (true)
					{
						try
						{
							string response = ReceiveMessage(stream);
							string decrypted = EncryptionHelper.Decrypt(response);

							if (decrypted == "END") break;
							Console.WriteLine($"Received: {decrypted}");
						}
						catch
						{
							Console.WriteLine("Connection closed or error.");
							return;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\nCould not connect to server: {ex.Message}\nPlease check the IP and port and try again.\n");
			}
		}

		Console.WriteLine("Client closed.");
	}

	static string ReceiveMessage(NetworkStream stream)
	{
		byte[] lengthBuffer = new byte[4];
		stream.Read(lengthBuffer, 0, 4);
		int length = BitConverter.ToInt32(lengthBuffer, 0);
		byte[] buffer = new byte[length];
		stream.Read(buffer, 0, length);
		return Encoding.UTF8.GetString(buffer);
	}
}
