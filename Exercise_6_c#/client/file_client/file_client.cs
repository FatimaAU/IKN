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
			//new file_client(args);

			System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
			clientSocket.Connect("10.0.0.1", PORT);

			//Send
			string testmsg = "Test";
			NetworkStream serverStream = clientSocket.GetStream();
			byte[] outStream = System.Text.Encoding.ASCII.GetBytes(testmsg + "$");
			serverStream.Write(outStream, 0, outStream.Length);
			serverStream.Flush();
			//			//Receive
			//			byte[] inStream = new byte[10025];
			//			serverStream.Read(inStream, 0, inStream.Length);
			//			string returndata = System.Text.Encoding.ASCII.GetString(inStream);
			//			returndata = returndata.Substring (0, returndata.IndexOf ("$"));
		}
	}
}
