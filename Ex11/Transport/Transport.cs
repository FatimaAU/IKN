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
			//Seq
			buffer [(int)TransCHKSUM.SEQNO] = seqNo;
			//Type
			buffer [(int)TransCHKSUM.TYPE] = (int)TransType.DATA;

			Array.Copy(buf, 0, buffer, 4, buf.Length);

			//Tilføjer de to første "bytes" på buf
			checksum.calcChecksum (ref buffer, buffer.Length);

			//Sat til at blive ved med løkken indtil den får det korrekt
			while (errorCount < 5) 
			{
				try
				{
					link.send(buffer, size+4);		

					seqNo = receiveAck ();

					if (old_seqNo == seqNo) 
					{
						Console.WriteLine ($"Error: Did not receive correctly (old_seqNo: {old_seqNo}, current seqNo: {seqNo})\n");
						errorCount++;
						//link.send (buf, size);
					} 
					else
					{
						Console.WriteLine ($"Received correctly (old_seqno: {old_seqNo}, current seqNo: {seqNo})\n");
						buffer [(int)TransCHKSUM.SEQNO] = (byte)((seqNo + 1) % 2);
						old_seqNo = seqNo;
						errorCount = 0;
						break;
					} 
				}
				catch 
				{
					Console.WriteLine ("Timed out, resending\n");
					errorCount++;
					//link.send (buf, size);
				}
			//old_seqNo = DEFAULT_SEQNO; //Vil ændre retning i applikationslageret åbenbart
			}

			Console.WriteLine ($"Ended transport session with {errorCount} errors\n");
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
			//byte[] temp = new byte[1000+(int)TransSize.ACKSIZE];

			//Receives data with full header
			recvSize = link.receive (ref buffer);

			Console.WriteLine ("recvsize: " + recvSize);
			//Do some checking on Seq and Type

			//Create new array with only checksum and data
			//byte[] checksumAndData = new byte[recvSize - 2];


			//checksumAndData = receivedWithHeader.To
			//Copy checksum from receivedWithHeader to checksumAndData
			//Array.Copy (receivedWithHeader, 0, checksumAndData, 0, 2);

			//Copy data from receivedWithHeader to checksumAndData
			//Array.Copy(receivedWithHeader, 4, checksumAndData, 2, receivedWithHeader.Length);

			//Console.WriteLine (LIB.ToString (checksumAndData));

			//Check data for corret checksum
			if (checksum.checkChecksum (buffer, recvSize)) 
			{
				sendAck (true);
				Console.WriteLine ("sending ack");
			} 
			else
				Console.WriteLine ("Error");

			return recvSize;
		}
	}
}