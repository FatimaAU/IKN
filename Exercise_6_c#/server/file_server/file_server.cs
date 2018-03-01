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
			// Starts server and accepts a client

			TcpListener serverSocket = new TcpListener(IPAddress.Any,PORT);
			TcpClient clientSocket = default(TcpClient);
			serverSocket.Start();
			clientSocket = serverSocket.AcceptTcpClient();
			Console.WriteLine("Accepted connection from client");

			NetworkStream clientStream = clientSocket.GetStream();
			string filename = LIB.readTextTCP (clientStream);
			Console.WriteLine ("Filename: " + filename);

			string path = "/root/Desktop/Exercise_6_c#/Exercise_6_c#/file_server/";

			long fileSize = LIB.check_File_Exists (path + filename);

			if (fileSize == 0)
			{
				string errorMsg = "File '" + filename + "' not found at '" + path + "'";
				Console.WriteLine(errorMsg);
				LIB.writeTextTCP (clientStream, fileSize.ToString());
				return;
				//string newFilename = LIB.readTextTCP (clientStream);
			} 

			Console.WriteLine("Size: " + fileSize);
			LIB.writeTextTCP (clientStream, fileSize.ToString());

			sendFile (filename, fileSize, clientStream);



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

			FileStream fs = new FileStream ("/root/Desktop/Exercise_6_c#/Exercise_6_c#/file_server/" + fileName, FileMode.Open, FileAccess.Read);
			Byte[] fileToSend = new Byte[BUFSIZE]; //Changed to bufsize maks send

			int bytesToSend;

			while ((bytesToSend = fs.Read (fileToSend, 0, fileToSend.Length)) > 0) //I exist to keep sending bytes until I only got 0 bytes to send left 
			{
				
				io.Write (fileToSend, 0, bytesToSend); //I must send that byte

				Console.WriteLine ($"send {bytesToSend} bytes");
			}
			//fs.Read (fileToSend, 0, bytesToSend); //I read the bytes but why should I read stuff at the server?

			Console.WriteLine (">>File sent");

			//fileToSend = File.ReadAllBytes ("/root/Documents/IKN/Exercise_6_c#/" + fileName);
			//io.Write (fileToSend, 0, (int)fileSize);
			//io.Write
			//while((
			// TO DO Your own code
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
			new file_server();

//				try
//				{
//					NetworkStream networkStream = clientSocket.GetStream();
//					byte[] bytesFrom = new byte[10025];
//					networkStream.Read(bytesFrom, 0, bytesFrom.Length);
//					string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
//					dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("0"));
//					//dataFromClient = dataFromClient.Substring(0, 1);
//					Console.WriteLine(" >> Data from client - " + dataFromClient);
//
//					string serverResponse = "Last Message from client" + dataFromClient;
//					Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
//					networkStream.Write(sendBytes, 0, sendBytes.Length);
//					networkStream.Flush();
//					Console.WriteLine(" >> " + serverResponse);
//				}
//				catch (Exception ex)
//				{
//					Console.WriteLine(ex.ToString());
//				}
//
//
//			clientSocket.Close();
//			serverSocket.Stop();
			Console.WriteLine(" >> exit");
			Console.ReadLine();
		}

	}
}


