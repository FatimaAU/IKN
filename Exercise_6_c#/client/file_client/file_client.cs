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
				Console.WriteLine ("Please supply two arguments: IP-address of the server and path + filename");
				Environment.Exit (0);
			}

			clientSocket.Connect(args[0], PORT);
			Console.WriteLine ($"Connected to '{args[0]}'");

			NetworkStream serverStream = clientSocket.GetStream();

			Console.WriteLine($"Requesting filename '{args[1]}'");

			string filename = args [1];
			LIB.writeTextTCP (serverStream, filename); 

			long fileSize = LIB.getFileSizeTCP (serverStream);

			while (fileSize == 0) 
			{
				Console.WriteLine ("File not found. Input a valid file");
				filename = Console.ReadLine ();

				Console.WriteLine($"Requesting filename '{filename}'");

				LIB.writeTextTCP (serverStream, filename);
				fileSize = LIB.getFileSizeTCP (serverStream);
			}
				
			Console.WriteLine ("File size: " + fileSize);

			receiveFile (filename, serverStream, fileSize);
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
		private void receiveFile (String fileName, NetworkStream io, long fileSize)
		{
			fileName = LIB.extractFileName(fileName);
			string dataDir = "/root/Desktop/ExFiles/";
			Directory.CreateDirectory (dataDir);
			FileStream file = new FileStream (dataDir + fileName, FileMode.Create, FileAccess.Write);
			byte[] data = new byte[BUFSIZE]; //Vi modtager kun 1k bytes af gangen


			int totalBytes = 0;
			int bytesRead;

			Console.WriteLine ("Reading file " + fileName + " ... ");

			//while ((bytesRead = io.Read(data, 0, data.Length)) > 0) //Nu bliver den ved indtil længden af det den modtager er 0
			while(fileSize > totalBytes)
			{
				bytesRead = io.Read (data, 0, data.Length);
				file.Write (data, 0, bytesRead);

				totalBytes += bytesRead;

				Console.WriteLine ("Read bytes: " + bytesRead.ToString () + "\t Total bytes read:" + totalBytes);

			}
				
			Console.WriteLine ("File received");
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
		}
	}
}
