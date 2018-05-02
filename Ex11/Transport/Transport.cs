using System;
using Linklaget;

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
			errorCount = 0;
			dataReceived = false;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private byte receiveAck()
		{
			recvSize = link.receive(ref buffer);
			dataReceived = true;

			if (recvSize == (int)TransSize.ACKSIZE) {
				dataReceived = false;
				if (!checksum.checkChecksum (buffer, (int)TransSize.ACKSIZE) ||
				  buffer [(int)TransCHKSUM.SEQNO] != seqNo ||
				  buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				{
					seqNo = (byte) buffer[(int)TransCHKSUM.SEQNO];
				}
				else
				{
					seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
				}
			}
 
			return seqNo;
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
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);
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
			buffer = buf;

			buffer [(int)TransCHKSUM.SEQNO] = seqNo;
			buffer [(int)TransCHKSUM.TYPE] = TransType.DATA;

			//Tilføjer de to første "bytes" på buf
			checksum.calcChecksum (ref buffer, size);

			while (errorCount < 5) 
			{
				link.send(buffer, size);

				seqNo = receiveAck ();

				if (old_seqNo ==  seqNo) 
				{
					Console.WriteLine ($"Error: Did not receive correctly at old_seqNo: {old_seqNo}, current seqNo: {seqNo}\n");
					errorCount++;
					link.send (buf, size);
				} 
				else if (old_seqNo != seqNo) 
				{
					Console.WriteLine ($"Received correctly with old_seqno: {old_seqNo}, current seqNo: {seqNo}\n");
					buffer [(int)TransCHKSUM.SEQNO] = (seqNo + 1) % 2;
					old_seqNo = seqNo;
					errorCount = 0;
				} 
				else 
				{
					Console.WriteLine ("Timed out, resending\n");
					errorCount++;
					link.send (buf, size);
				}
			}

			Console.WriteLine ($"Ended transport session with {errorCount} errors)");
			Console.ReadLine ();
		}

		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			
			//Receives data with full header
			//link.receive
			link.receive (ref buf);

			//Check data for corret checksum
			if(checksum.checkChecksum (buf))
			{
				sendAck(true);
			}
//			else
//			// TO DO Your own code
			return link.receive (ref buf);
		}
	}
}