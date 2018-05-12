using System;
using Linklaget;
using System.Text;
using Library;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{
		/// <summary>
		/// The link.
		/// </summary>
		private Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte old_seqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;
		/// <summary>
		/// The data received. True = received data in receiveAck, False = not received data in receiveAck
		/// </summary>
		private bool dataReceived;
		/// <summary>
		/// The number of data the recveived.
		/// </summary>
		private int recvSize = 0;
		private byte ack_seqNo;
		private int transmitCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE, string APP)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			ack_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
			transmitCount = 0;
			dataReceived = false;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private void receiveAck()
		{
			recvSize = link.receive(ref buffer);
			dataReceived = true;

			if (recvSize == (int)TransSize.ACKSIZE) {
				dataReceived = false;
				if (!checksum.checkChecksum (buffer, (int)TransSize.ACKSIZE)
					|| buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				{
					Console.WriteLine("Error in ack checksum or type!");
					ack_seqNo = (byte) ((ack_seqNo + 1) % 2); // Increment current ack_seq
				}
				else
				{
					ack_seqNo = (byte) buffer[(int)TransCHKSUM.SEQNO]; // No incrementing since already incremented
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
				(byte)(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);

			if(++transmitCount == 1) // Simulate noise
			{
				ackBuf[1]++; // Important: Only spoil a checksum-field (ackBuf[0] or ackBuf[1])
				Console.WriteLine($"Noise! ack #{transmitCount} is spoiled in a transmitted ACK-package");
			}

			if (transmitCount == 10)
				transmitCount = 0;

			Console.WriteLine ($"Ack sends seqNo {ackBuf[(int)TransCHKSUM.SEQNO]}");

			link.send(ackBuf, (int)TransSize.ACKSIZE);
		}

		/// <summary>
		/// Send the specified buffer and size.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send(byte[] buf, int size)
		{
			do
			{
				ack_seqNo = seqNo;
				//Seq
				buffer [(int)TransCHKSUM.SEQNO] = seqNo;
				//Type
				buffer [(int)TransCHKSUM.TYPE] = (int)TransType.DATA;

				Array.Copy(buf, 0, buffer, 4, buf.Length);

				//Tilføjer de to første "bytes" på buf
				checksum.calcChecksum (ref buffer, buffer.Length);
				//buffer [1]++;

				if(++transmitCount == 3) // Simulate noise
				{
					buffer[1]++; // Important: Only spoil a checksum-field (buffer[0] or buffer[1])
					Console.WriteLine($"Noise! - pack #{transmitCount} is spoiled");
				}
				if (transmitCount == 10)
					transmitCount = 0;

				try
				{
					Console.WriteLine("Sending pack");

					link.send(buffer, size+4);	

					Console.WriteLine("Receiving ack");

					receiveAck ();

					if (ack_seqNo != seqNo) 
					{
						Console.WriteLine ($"Error: Did not receive correctly (ack_seqNo: {ack_seqNo}, current seqNo: {seqNo})\n");
						Console.WriteLine("Resending same package");
					} 
					else if (ack_seqNo == seqNo)
					{
						Console.WriteLine ($"Received correctly (ack_seqNo: {ack_seqNo}, current seqNo: {seqNo})\n");
						break;
					}
					else
						Console.WriteLine("some other error");
				}
				catch(TimeoutException) 
				{
					Console.WriteLine ("Timed out, resending\n");
				}

				errorCount++;
				Console.WriteLine ("Errorcount: " + errorCount);

			}while ((errorCount < 5) || (seqNo != ack_seqNo));
				
			//old_seqNo = DEFAULT_SEQNO; //Vil ændre retning i applikationslageret åbenbart
			seqNo = (byte)((seqNo + 1) % 2);
			errorCount = 0;
			ack_seqNo = DEFAULT_SEQNO;
		}

		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			recvSize = 0;

			// Reset buffer
			for(int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = 0;
			}

			while(recvSize == 0)
			{
				try
				{
					recvSize = link.receive(ref buffer);	//returns length of received byte array
				} 
				catch(Exception)
				{
					Console.WriteLine ("Transport receive timed out");
				}
			}
			if (checksum.checkChecksum (buffer, recvSize)) 
			{
				Console.WriteLine ("Data pack OK.");

				seqNo = buffer [(int)TransCHKSUM.SEQNO];

				if (seqNo != old_seqNo) 
					Array.Copy (buffer, (int)TransSize.ACKSIZE, buf, 0, recvSize - (int)TransSize.ACKSIZE);
				else
					Console.WriteLine ("Received identical package. Ignore");

				old_seqNo = seqNo;
				sendAck (true);
				return recvSize - 4;
			} 
			else
			{
				Console.WriteLine ("Error in data pack. Sending NACK.");
				sendAck (false);
				return 0;
			}


		}
	}
}