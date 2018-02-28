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

			string path = "/root/Documents/IKN/Exercise_6_c#/";

			if (LIB.check_File_Exists(path + filename) == 0)
			{
				string errorMsg = "File '" + filename + "' not found at '" + path + "'";
				Console.WriteLine(errorMsg);
				LIB.writeTextTCP (clientStream, errorMsg);
				
			} 
			else 
			{
				long fileSize = new System.IO.FileInfo (path + filename).Length;
				string fileMsg = "Size: " + fileSize;

				Console.WriteLine(fileMsg);
				LIB.writeTextTCP (clientStream, fileMsg);
			}
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
			Byte[] fileToSend = new Byte[fileSize];
			FileStream fs = File.Open ("/root/Documents/IKN/Exercise_6_c#/" + fileName);								
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


