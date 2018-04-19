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

    		return (~((sum & 0xFFFF)+(sum >> 16)))&0xFFFF;
		}

		public bool checkChecksum(byte[] buf, int size)
		{
			byte[] buffer = new byte[size-2];

			Array.Copy(buf, (int)TransSize.CHKSUMSIZE, buffer, 0, buffer.Length);
			return( checksum(buffer) == (long)(buf[(int)TransCHKSUM.CHKSUMHIGH] << 8 | buf[(int)TransCHKSUM.CHKSUMLOW]));
		}

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