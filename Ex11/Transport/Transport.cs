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
					_ackSeqNo = (byte) ((_buffer[(int)TransCHKSUM.SEQNO] + 1) % 2); // Increment current ack_seq
				}
				else
				{
					_ackSeqNo = (byte) _buffer[(int)TransCHKSUM.SEQNO]; // No incrementing since already incremented
				}
			}
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
				(byte)(ackType ? (byte)_buffer [(int)TransCHKSUM.SEQNO] 
					: (byte)(_buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.CalcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);

//			if(_transmitCount == 1) // Simulate noise
//			{
//				ackBuf[1]++; // Important: Only spoil a checksum-field (ackBuf[0] or ackBuf[1])
//				Console.WriteLine($"Noise! ack #{_transmitCount} checksum is spoiled in a transmitted ACK-package");
//			}

			if (_transmitCount == 10)
				_transmitCount = 0;

			Console.WriteLine ($"Ack sends seqNo #{ackBuf[(int)TransCHKSUM.SEQNO]}");

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
			// Reset buffer

			do
			{
				for (int i = 0; i < _buffer.Length; i++) 
				{
					_buffer [i] = 0;
				}

				//Seq
				_buffer [(int)TransCHKSUM.SEQNO] = _seqNo;
				//Type
				_buffer [(int)TransCHKSUM.TYPE] = (int)TransType.DATA;

				Array.Copy(buf, 0, _buffer, 4, size);

				//Tilføjer de to første "bytes" på buf
				checksum.CalcChecksum (ref _buffer, _buffer.Length);

				Console.WriteLine($"TRANSMIT #{++_transmitCount}");

				if(_transmitCount == 3) // Simulate noise
				{
					_buffer[1]++; // Important: Only spoil a checksum-field (buffer[0] or buffer[1])
					Console.WriteLine($"Noise! - pack #{_transmitCount} is spoiled");
				}

				if (_transmitCount == 5)
					_transmitCount = 0;

				_ackSeqNo = _seqNo;
				try
				{
					Console.WriteLine($"Sending pack with seqNo #{_seqNo}");

					link.Send(_buffer, size+4);

					// Receive the ack from the receiver
                    receiveAck ();

				    Console.WriteLine($"Receiving ack with seqNo #{_ackSeqNo}");

					// If the ack is not equal to seqNo
					//, the package is not received correctly
					if (_ackSeqNo != _seqNo) 
					{
						Console.WriteLine ("\tError: Did not receive correctly");
						Console.WriteLine("\t\tResending same package");
					} 
					else
					{
						Console.WriteLine ("\tReceived correctly\n");
						break;
					}
				}
				catch(TimeoutException) 
				{
					Console.WriteLine ("\tTimed out, resending");
				}

				_errorCount++;
				Console.WriteLine ("\tErrorcount: " + _errorCount + "\n");

			}while ((_errorCount < 10));

			if (_errorCount >= 10) 
			{
				Console.WriteLine ("With errorcount " + _errorCount + ", I am out.");
				Environment.Exit (1);
			}

			// Update seqNo
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
			// Reset buffer
			for (int i = 0; i < _buffer.Length; i++) {
				_buffer [i] = 0;
			}

			while (true) 
			{
				_recvSize = 0;
				// Will time out while waiting, so must catch 
				while (_recvSize == 0 || _recvSize == -1) 
				{
					try 
					{
						_recvSize = link.Receive (ref _buffer);	//returns length of received byte array
						if (_recvSize == -1)
						{
							// Send NACK and wait for message to be done sending
							sendAck(false); 
							System.Threading.Thread.Sleep(200);
						}
					} 
					catch (Exception) 
					{
					}
				}

				Console.WriteLine ($"TRANSMIT #{++_transmitCount}");

				// If check is not right, send NACK, else do this
				if (checksum.CheckChecksum (_buffer, _recvSize)) 
				{
					Console.WriteLine ("Data pack OK.");

					// Update seqNo
					_seqNo = _buffer [(int)TransCHKSUM.SEQNO];

					// Return the received data
					Array.Copy (_buffer, (int)TransSize.ACKSIZE, buf, 0, _recvSize - (int)TransSize.ACKSIZE);

					// If identical package received, ignore
					if (_seqNo == _oldSeqNo)
						Console.WriteLine ("\tReceived identical package. Ignore");

					// Update oldSeqNo and return ACK
					_oldSeqNo = _seqNo;
					sendAck (true);
					return _recvSize - 4;
				}

				Console.WriteLine ("Error in data pack. Sending NACK.");
				sendAck (false);
			}
		}
	}
}