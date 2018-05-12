using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace LinkLayer
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
		private readonly byte[] _buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		private readonly SerialPort _serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					_serialPort = new SerialPort("/dev/tnt1",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					_serialPort = new SerialPort("/dev/tnt0",115200,Parity.None,8,StopBits.One);
				}
			#else
				serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			#endif
			if(!_serialPort.IsOpen)
				_serialPort.Open();

			_buffer = new byte[(BUFSIZE*2) + 4];

			// Uncomment the next line to use timeout
			_serialPort.ReadTimeout = 500;
			_serialPort.DiscardInBuffer();
			_serialPort.DiscardOutBuffer();
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
		public void Send (byte[] buf, int size)
		{
			// Store SLIP
		    var data = new List<byte> {DELIMITER};

		    // Delimiter is appended

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

			_serialPort.Write (data.ToArray(), 0, data.Count);
		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		public int Receive (ref byte[] buf)
		{
			int size = _serialPort.Read(_buffer, 0, _buffer.Length);
				
			int bufIndex = 0;
			// Make sure first index is DELIMITER
			if (_buffer [0] == DELIMITER) 
			{

				// Loop through from next index
				for (int i = 1; i < size; i++) 
				{

					if (_buffer [i] == 'B') 
					{
						// Must check on next index to insert A or B
						switch (_buffer [i + 1]) 
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
					else if (_buffer [i] == DELIMITER)
						break;
					else 
						buf [bufIndex++] = _buffer [i];
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
