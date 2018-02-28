using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


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
			// TO DO Your own code
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

			TcpListener serverSocket = new TcpListener(IPAddress.Any,PORT);
			int requestCount = 0;
			TcpClient clientSocket = default(TcpClient);
			serverSocket.Start();
			Console.WriteLine(" >> Server Started");
			clientSocket = serverSocket.AcceptTcpClient();
			Console.WriteLine(" >> Accept connection from client");
			requestCount = 0;

			//while ((true))
			{
				try
				{
					requestCount = requestCount + 1;
					NetworkStream networkStream = clientSocket.GetStream();
					byte[] bytesFrom = new byte[10025];
					networkStream.Read(bytesFrom, 0, bytesFrom.Length);
					string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
					dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
					//dataFromClient = dataFromClient.Substring(0, 1);
					Console.WriteLine(" >> Data from client - " + dataFromClient);

					string serverResponse = "Last Message from client" + dataFromClient;
					Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
					networkStream.Write(sendBytes, 0, sendBytes.Length);
					networkStream.Flush();
					Console.WriteLine(" >> " + serverResponse);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}

			clientSocket.Close();
			serverSocket.Stop();
			Console.WriteLine(" >> exit");
			Console.ReadLine();
		}

	}
}


