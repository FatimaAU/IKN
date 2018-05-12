using System;
using System.Text;
using TransportLayer;

namespace server
{
	class file_server
	{
		
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";
		Transport transport = new Transport (BUFSIZE, APP);

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			sendFile ("dd", 33, transport);
			// TO DO Your own code
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			Console.WriteLine ("Sending AXBY in file server\n");
			//string inString = Console.ReadLine ();
			string inString = "AXBY";
			byte[] toSend = Encoding.ASCII.GetBytes (inString);
			//Console.WriteLine ("Sending to transport");
			transport.Send (toSend, toSend.Length);
			transport.Send (toSend, toSend.Length);
			transport.Send (toSend, toSend.Length);
			transport.Send (toSend, toSend.Length);
			transport.Send (toSend, toSend.Length);
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
			new file_server();
		}
	}
}