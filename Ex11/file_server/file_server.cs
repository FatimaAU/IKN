using System;
using System.Text;
using TransportLayer;
using Library;
using System.IO;

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

			// Get filename with size of byte 
		    int size = _transport.Receive(ref filename);

			// Extract file name from the path
			string filenameStr = LIB.ExtractFileName (LIB.ToString (filename).Substring (0, size));

			Console.WriteLine($"\nFilename {LIB.ToString(filename)}");

			// Check file exist
			long fileSize = LIB.check_File_Exists(LIB.ToString(filename).Substring(0, size));

			Console.WriteLine (LIB.ToString (filename).Substring (0, size));

			// If it does not exist, ask for another filename
		    while (fileSize == 0)
		    {
				string errorMsg = "File '" + LIB.ToString(filename) + "' not found \n";

		        Console.WriteLine(errorMsg);

				// Send filesize back
				_transport.Send(LIB.ToBytes(fileSize.ToString()), LIB.ToBytes(fileSize.ToString()).Length);

		        size =_transport.Receive(ref filename);

				Console.WriteLine (LIB.ToString (filename));

				// Check if file exist - must do with substring to get exact path
				fileSize = LIB.check_File_Exists(LIB.ToString(filename).Substring(0, size));
		    }

            Console.WriteLine("File is found with size " + fileSize);

			_transport.Send(LIB.ToBytes(fileSize.ToString()), LIB.ToBytes(fileSize.ToString()).Length);

			sendFile (LIB.ToString (filename).Substring(0, size), fileSize, _transport);
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
			Console.WriteLine ("Sending file ..");

			FileStream fs = new FileStream (fileName, FileMode.Open, FileAccess.Read);
			byte[] fileToSend = new byte[BUFSIZE]; //Changed to bufsize maks send6

			int bytesToSend = 0;

			while ((bytesToSend = fs.Read (fileToSend, 0, fileToSend.Length)) > 0) //I exist to keep sending bytes until I only got 0 bytes to send left 
			{
				_transport.Send (fileToSend, bytesToSend);
				Console.WriteLine ($"Sent {bytesToSend} bytes");

			}

			Console.WriteLine ("File sent");
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			while(true)
				new file_server();
				
		}
	}
}