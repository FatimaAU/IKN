using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp
{
	class file_client
	{
		/// <summary>
		/// The PORT.
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments. First ip-adress of the server. Second the filename
		/// </param>
		private file_client (string[] args)
		{
			// TO DO Your own code

			//char nullTerm = '0';

			System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();

			if (args.Length != 2) 
			{
				Console.WriteLine ("Please supply two arguments: IP-address of the server and filename");
				Environment.Exit (0);
			}

			clientSocket.Connect(args[0], PORT);
			Console.WriteLine ($"Connected to \"{args[0]}\"");

			NetworkStream serverStream = clientSocket.GetStream();

			Console.WriteLine($"Requesting filename \"{args[1]}\"");
			LIB.writeTextTCP (serverStream, args [1]); 

			string msg = LIB.readTextTCP (serverStream);
			Console.WriteLine (msg);

			//Send
//			string testmsg = "Test";
//			NetworkStream serverStream = clientSocket.GetStream();
//			byte[] outStream = System.Text.Encoding.ASCII.GetBytes(testmsg + "$");
//			serverStream.Write(outStream, 0, outStream.Length);
//			serverStream.Flush();
		}

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='io'>
		/// Network stream for reading from the server
		/// </param>
		private void receiveFile (String fileName, NetworkStream io)
		{
			// TO DO Your own code
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");
			new file_client(args);
//			System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
//			clientSocket.Connect("10.0.0.1", PORT);
			///Console.WriteLine (args[1]);
////
////			//Send
//			string testmsg = "Test";
//			NetworkStream serverStream = clientSocket.GetStream();
//			byte[] outStream = System.Text.Encoding.ASCII.GetBytes(testmsg + "$");
//			serverStream.Write(outStream, 0, outStream.Length);
//			serverStream.Flush();

			//			//Receive
			//			byte[] inStream = new byte[10025];
			//			serverStream.Read(inStream, 0, inStream.Length);
			//			string returndata = System.Text.Encoding.ASCII.GetString(inStream);
			//			returndata = returndata.Substring (0, returndata.IndexOf ("$"));
		}
	}
}
