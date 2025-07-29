<h2> Socket-Programming </h2>
<br> Encrypted Client-Server Application (C#) This is a secure client-server communication project built with C#. It uses TCP sockets and custom encryption to exchange messages between the server and the client.

Features TCP-based communication using .NET Sockets

Symmetric encryption and decryption of all messages

Predefined dictionary-based data lookup

Client sends main-sub key combination and receives timestamps if matched

Graceful handling of disconnects and invalid input

Project Structure Server.cs – Main server logic

Client.cs – Main client logic

EncryptionHelper.cs – Helper class for encrypting/decrypting strings

README.md – This file

How It Works The server listens on a port entered by the user.

The client connects by entering the IP and port of the server.

The client sends an encrypted message (e.g., SetA-One).

The server decrypts it, validates it using a nested dictionary, and responds with two timestamps and an "END" signal.

Both client and server support graceful disconnection with exit command.

How to Run Clone the repository:

git clone [https://github.com/deekshita-nirmal12/socket-programming](https://github.com/deekshita-nirmal12/socket-programming)
<br> cd socket-programming Open the solution/project in Visual Studio 2022.

Build the solution.

Run Server and Client Project

Server: Enter a port number to start listening.

Client: Enter the IP address (e.g., 127.0.0.1) and port number to connect.

Example Interaction Client Input: SetA-One Server Response: [Timestamp 1] [Timestamp 2]

Note The dictionary keys are case-insensitive.

Encryption used here is for demonstration; for production use, consider more secure libraries like AES from System.Security.Cryptography.
