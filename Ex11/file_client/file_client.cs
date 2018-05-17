using System;
using System.Text;
using System.IO;
using Library;
using TransportLayer;

namespace client
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";
		private Transport _transport = new Transport(BUFSIZE, APP);

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(string[] args)
	    {
	        if (args.Length != 1)
	        {
	            Console.WriteLine("Please supply a filename");
	            Environment.Exit(0);
            }

			// Get filename
	        string filename = args[0];

			Console.WriteLine ("Requesting filename " + args [0] + "\n");

			// Send file name 
			_transport.Send(LIB.ToBytes(filename), LIB.ToBytes(filename).Length);

			// Filesize in bytes
			byte[] fileSize = new byte[BUFSIZE];

			// Receive file s
			int size = _transport.Receive(ref fileSize);

			while ((LIB.ToString(fileSize).Substring(0, size) == "0"))
	        {
                Console.WriteLine($"File {filename} not found. Input a valid file");
	            filename = Console.ReadLine();

	            Console.WriteLine($"Requesting filename '{filename}'");

				_transport.Send (LIB.ToBytes (filename), LIB.ToBytes (filename).Length);

				_transport.Receive(ref fileSize);
            }

			Console.WriteLine ("File size: " + LIB.ToString(fileSize));

			receiveFile (filename, long.Parse (LIB.ToString (fileSize)), _transport);
	    }
		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (string fileName, long fileSize, Transport transport)
		{
			string dataDir = "/root/Desktop/Ex11_TransmittedFiles/";
			Directory.CreateDirectory (dataDir);

			FileStream file = new FileStream (dataDir + fileName, FileMode.Create, FileAccess.Write);
			byte[] data = new byte[BUFSIZE]; //Vi modtager kun 1k bytes af gangen

			int totalBytes = 0;
			int bytesRead;

			Console.WriteLine ("Reading file " + fileName + " ... ");

			while(fileSize > totalBytes)
			{
				bytesRead = _transport.Receive (ref data);
				file.Write (data, 0, bytesRead);
				totalBytes += bytesRead;

				Console.WriteLine ("Read bytes: " + bytesRead + "\t Total bytes read:" + totalBytes);

			}

			Console.WriteLine ("File received");
			file.Close ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
			// Console.WriteLine("HANS"); // MUST NOT BE DELETED!!!!!
			Console.WriteLine ("Client starts..\n");
			new file_client (args);
		}
	}
}