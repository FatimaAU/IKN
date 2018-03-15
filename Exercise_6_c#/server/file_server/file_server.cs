using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace tcp
{
	class file_server
	{
		/// <summary>
		/// The PORT
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		const int BUFSIZE = 1000;
		int counter = 0;
		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// Opretter en socket.
		/// Venter på en connect fra en klient.
		/// Modtager filnavn
		/// Finder filstørrelsen
		/// Kalder metoden sendFile
		/// Lukker socketen og programmet
		/// </summary>
		private file_server ()
		{
			//string path = "/root/Desktop/Exercise_6_c#/Exercise_6_c#/file_server/";


			// Starts server and accepts a client
			Console.WriteLine("Listening for client ..");

			TcpListener serverSocket = new TcpListener(IPAddress.Any,PORT);
			TcpClient clientSocket = default(TcpClient);
			serverSocket.Start();
			clientSocket = serverSocket.AcceptTcpClient();

			Console.WriteLine("Accepted connection from client");

			// Get the network file stream
			NetworkStream clientStream = clientSocket.GetStream();

			string filename = LIB.readTextTCP (clientStream);
			long fileSize = LIB.check_File_Exists (filename);

			while (fileSize == 0)
			{
				string errorMsg = "File '" + filename + "' not found";
				Console.WriteLine(errorMsg);

				LIB.writeTextTCP (clientStream, fileSize.ToString());

				filename = LIB.readTextTCP (clientStream);
				fileSize = LIB.check_File_Exists (filename);

			} 

			Console.WriteLine ("Filename: " + filename);
			Console.WriteLine("Size: " + fileSize);
			LIB.writeTextTCP (clientStream, fileSize.ToString());

			sendFile (filename, fileSize, clientStream);

			clientSocket.Close();
			serverSocket.Stop();
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
			Console.WriteLine ("Sending file ..");

			fileSize = LIB.check_File_Exists (fileName);

			FileStream fs = new FileStream (fileName, FileMode.Open, FileAccess.Read);
			Byte[] fileToSend = new Byte[BUFSIZE]; //Changed to bufsize maks send6

			int bytesToSend = 0;

			while ((bytesToSend = fs.Read (fileToSend, 0, fileToSend.Length)) > 0) //I exist to keep sending bytes until I only got 0 bytes to send left 
			{
				
				io.Write (fileToSend, 0,bytesToSend); //I must send that byte

				Console.WriteLine ($"Send {bytesToSend} bytes");

			}
			counter++;
			Console.WriteLine ("File sent");
			Console.WriteLine($"Sent counter: {counter}");

		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		static void Main(string[] args)
		{
			Console.WriteLine ("Server starts...");
			while (true) 
			{
				new file_server ();
			}
		}

	}
}


