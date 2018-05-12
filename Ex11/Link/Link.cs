using System;
using System.IO.Ports;
using System.Text;
using Library;
using System.Collections.Generic;
/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					serialPort = new SerialPort("/dev/tnt1",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					serialPort = new SerialPort("/dev/tnt0",115200,Parity.None,8,StopBits.One);
				}
			#else
				serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			#endif
			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2) + 4];

			// Uncomment the next line to use timeout
			serialPort.ReadTimeout = 1500;
			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
			// Store SLIP
			List<byte> data = new List<byte>();

			// Delimiter is appended
			data.Add (DELIMITER);

			// Iterate through the data and append
			for(int i = 0; i < size; i++)
			{
				// Check if A or B - replace with BC/BD, else append
				if (buf [i] == 'A') 
				{
					data.Add ((byte)'B');
					data.Add ((byte)'C');
				}
				else if (buf [i] == 'B')
				{
					data.Add ((byte)'B');
					data.Add((byte)'D');
				}
				else
				{
					data.Add (buf [i]);
				}
			}

			data.Add (DELIMITER);

			serialPort.Write (data.ToArray(), 0, data.Count);
		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			int size = serialPort.Read (buffer, 0, buffer.Length);
				
			int bufIndex = 0;
			// Make sure first index is DELIMITER
			if (buffer [0] == DELIMITER) 
			{

				// Loop through from next index
				for (int i = 1; i < size; i++) 
				{

					if (buffer [i] == 'B') 
					{
						// Must check on next index to insert A or B
						switch (buffer [i + 1]) 
						{
						case (byte)'C':
							buf [bufIndex++] = (byte)'A';
							i++;
							break;
						case (byte)'D':
							buf [bufIndex++] = (byte)'B';	
							i++;
							break;
						}
					} 
					else if (buffer [i] == DELIMITER)
						break;
					else 
						buf [bufIndex++] = buffer [i];
				}
			} 
			else 
			{
				Console.WriteLine ("Did not receive correct delimiter. Exiting\n");
				Environment.Exit (1);
			}

			return bufIndex;
		}
	}
}
