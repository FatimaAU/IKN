using System;

namespace Transportlaget
{
	public class Checksum
	{
		public Checksum ()
		{
		}

		private long checksum (byte[] buf)
		{
    		int i = 0, length = buf.Length;
    		long sum = 0;
    		while (length > 0) 
			{
        		sum	+= (buf[i++]&0xff) << 8;
        		if ((--length)==0) break;
        		sum += (buf[i++]&0xff);
        		--length;
    		}

			Console.WriteLine ("sum: " + sum);

    		return (~((sum & 0xFFFF)+(sum >> 16)))&0xFFFF;
		}

		/// <summary>
		/// Check the checksum. Returns true on correct and false on incorrect.
		/// </summary>
		/// <param name='buf'>
		/// Buffer of the data + header for checksum.
		/// </param>
		/// <param name='size'>
		/// Size of buffer + header.
		/// </param>
		public bool checkChecksum(byte[] buf, int size)
		{
			byte[] buffer = new byte[size-4];

			Array.Copy(buf, (int)TransSize.CHKSUMSIZE, buffer, 0, buffer.Length);

			return( checksum(buffer) == (long)(buf[(int)TransCHKSUM.CHKSUMHIGH] << 8 | buf[(int)TransCHKSUM.CHKSUMLOW]));
		}

		/// <summary>
		/// Calculates the checksum.
		/// </summary>
		/// <param name='buf'>
		/// Buffer of the data + space for checksum.
		/// </param>
		/// <param name='size'>
		/// Size of buffer + checksumsize.
		/// </param>
		public void calcChecksum (ref byte[] buf, int size)
		{
			byte[] buffer = new byte[size-2];
			long sum = 0;

			Array.Copy(buf, 2, buffer, 0, buffer.Length);
			sum = checksum(buffer);

			buf[(int)TransCHKSUM.CHKSUMHIGH] = (byte)((sum >> 8) & 255);
			buf[(int)TransCHKSUM.CHKSUMLOW] = (byte)(sum & 255);
		}
	}
}