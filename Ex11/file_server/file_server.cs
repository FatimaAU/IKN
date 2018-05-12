using System;
using System.Text;
using TransportLayer;
using Library;

namespace server
{
	class file_server
	{
		
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";
	    private readonly Transport _transport = new Transport (BUFSIZE, APP);

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
		    Console.WriteLine("Waiting for client to supply filename \n");

            var filename = new byte[BUFSIZE];

		    _transport.Receive(ref filename);

		    string filenameInString = LIB.ToString(filename);

			Console.WriteLine($"\nFilename {filenameInString}");

		    long fileSize = LIB.check_File_Exists(filenameInString);

		    while (fileSize == 0)
		    {
		        string errorMsg = "File '" + filename + "' not found";
		        Console.WriteLine(errorMsg);

                var fileSizeToSend = LIB.ToBytes(fileSize.ToString());

                _transport.Send(fileSizeToSend, fileSizeToSend.Length);

		        _transport.Receive(ref filename);

		        fileSize = LIB.check_File_Exists(LIB.ToString(filename));
		    }

            Console.WriteLine("File is found with size " + fileSize);

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
		private void sendFile(string fileName, long fileSize, Transport transport)
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