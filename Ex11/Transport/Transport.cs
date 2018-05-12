using System;
using LinkLayer;

namespace TransportLayer
{
	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{
		/// <summary>
		/// The link.
		/// </summary>
		private readonly Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] _buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte _seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte _oldSeqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int _errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;
		/// <summary>
		/// The data received. True = received data in receiveAck, False = not received data in receiveAck
		/// </summary>
		private bool _dataReceived;
		/// <summary>
		/// The number of data received.
		/// </summary>
		private int _recvSize;
	    /// <summary>
	    /// The seq number of ack.
	    /// </summary>
		private byte _ackSeqNo;
        /// <summary>
		/// Counting amount of transmits.
		/// </summary>
		private int _transmitCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE, string APP)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
			checksum = new Checksum();
			_buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			_seqNo = 0;
			_oldSeqNo = DEFAULT_SEQNO;
			_ackSeqNo = DEFAULT_SEQNO;
			_errorCount = 0;
			_transmitCount = 0;
			_dataReceived = false;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private void receiveAck()
		{
			_recvSize = link.Receive(ref _buffer);
			_dataReceived = true;

			if (_recvSize == (int)TransSize.ACKSIZE) {
				_dataReceived = false;
				if (!checksum.CheckChecksum (_buffer, (int)TransSize.ACKSIZE)
					|| _buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				{
					Console.WriteLine("Error in ack checksum or type!");
					_ackSeqNo = (byte) ((_ackSeqNo + 1) % 2); // Increment current ack_seq
				}
				else
				{
					_ackSeqNo = (byte) _buffer[(int)TransCHKSUM.SEQNO]; // No incrementing since already incremented
				}
			}
 			
			//return ack_seqNo;
		}

		/// <summary>
		/// Sends the ack.
		/// </summary>
		/// <param name='ackType'>
		/// Ack type.
		/// </param>
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = 
				(byte)(ackType ? (byte)_buffer [(int)TransCHKSUM.SEQNO] : (byte)(_buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.CalcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);

			if(++_transmitCount == 1) // Simulate noise
			{
				ackBuf[1]++; // Important: Only spoil a checksum-field (ackBuf[0] or ackBuf[1])
				Console.WriteLine($"Noise! ack #{_transmitCount} is spoiled in a transmitted ACK-package");
			}

			if (_transmitCount == 10)
				_transmitCount = 0;

			Console.WriteLine ($"Ack sends seqNo {ackBuf[(int)TransCHKSUM.SEQNO]}");

			link.Send(ackBuf, (int)TransSize.ACKSIZE);
		}

	    /// <summary>
	    /// Send the specified buffer and size.
	    /// </summary>
	    /// <param name="buf">
	    /// Buffer.
	    /// </param>
	    /// <param name='size'>
	    /// Size.
	    /// </param>
	    public void Send(byte[] buf, int size)
		{
			do
			{
				_ackSeqNo = _seqNo;
				//Seq
				_buffer [(int)TransCHKSUM.SEQNO] = _seqNo;
				//Type
				_buffer [(int)TransCHKSUM.TYPE] = (int)TransType.DATA;

				Array.Copy(buf, 0, _buffer, 4, buf.Length);

				//Tilføjer de to første "bytes" på buf
				checksum.CalcChecksum (ref _buffer, _buffer.Length);
				//buffer [1]++;

				if(++_transmitCount == 3) // Simulate noise
				{
					_buffer[1]++; // Important: Only spoil a checksum-field (buffer[0] or buffer[1])
					Console.WriteLine($"Noise! - pack #{_transmitCount} is spoiled");
				}
				if (_transmitCount == 10)
					_transmitCount = 0;

				try
				{
					Console.WriteLine($"Sending pack with seqNo #{_seqNo}");

					link.Send(_buffer, size+4);

                    receiveAck ();

				    Console.WriteLine($"Receiving ack with seqNo #{_ackSeqNo}");

					if (_ackSeqNo != _seqNo) 
					{
						Console.WriteLine ("\tError: Did not receive correctly");
						Console.WriteLine("\t\tResending same package");
					} 
					else if (_ackSeqNo == _seqNo)
					{
						Console.WriteLine ("\tReceived correctly\n");
						break;
					}
					else
						Console.WriteLine("\tsome other error");
				}
				catch(TimeoutException) 
				{
					Console.WriteLine ("\tTimed out, resending");
				}

				_errorCount++;
				Console.WriteLine ("\tErrorcount: " + _errorCount + "\n");

			}while ((_errorCount < 5) || (_seqNo != _ackSeqNo));
				
			//old_seqNo = DEFAULT_SEQNO; //Vil ændre retning i applikationslageret åbenbart
			_seqNo = (byte)((_seqNo + 1) % 2);
			_errorCount = 0;
			_ackSeqNo = DEFAULT_SEQNO;
		}

        /// <summary>
        /// Receive the specified buffer.
        /// </summary>
        /// <param name="buf">
        /// Buffer.
        /// </param>
        public int Receive (ref byte[] buf)
		{
			_recvSize = 0;

			// Reset buffer
			for(int i = 0; i < _buffer.Length; i++)
			{
				_buffer[i] = 0;
			}

			while(_recvSize == 0)
			{
				try
				{
					_recvSize = link.Receive(ref _buffer);	//returns length of received byte array
				} 
				catch(Exception)
				{
					Console.WriteLine ("Transport receive timed out");
				}
			}
			if (checksum.CheckChecksum (_buffer, _recvSize)) 
			{
				Console.WriteLine ("Data pack OK.");

				_seqNo = _buffer [(int)TransCHKSUM.SEQNO];

				if (_seqNo != _oldSeqNo) 
					Array.Copy (_buffer, (int)TransSize.ACKSIZE, buf, 0, _recvSize - (int)TransSize.ACKSIZE);
				else
					Console.WriteLine ("Received identical package. Ignore");

				_oldSeqNo = _seqNo;
				sendAck (true);
				return _recvSize - 4;
			}

		    Console.WriteLine ("Error in data pack. Sending NACK.");
		    sendAck (false);
		    return 0;
		}
	}
}