using System;
using System.Text;
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

	        string filename = args[0];

            var filenameInBytes = LIB.ToBytes(args[0]);

			Console.WriteLine ("Requesting filename " + args [0] + "\n");

            _transport.Send(filenameInBytes, filename.Length);

            var fileSize = new byte[BUFSIZE];

	        long size = _transport.GetFileSize(ref fileSize);

	        while (size == 0)
	        {
                Console.WriteLine($"File {filename} not found. Input a valid file");
	            filename = Console.ReadLine();

	            Console.WriteLine($"Requesting filename '{filename}'");

            }
	        // TO DO Your own code
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
		private void receiveFile (string fileName, Transport transport)
		{
            Console.WriteLine($"Requesting filename {fileName}");

			while(true)
			{
				byte[] received = new byte[BUFSIZE];
				int size = transport.Receive (ref received);
				
				string receivedInString = Encoding.ASCII.GetString (received);
				
				Console.WriteLine ("Received string " + receivedInString + " with size " + size + "\n");
			}
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts..\n");
			new file_client (args);
		}
	}
}