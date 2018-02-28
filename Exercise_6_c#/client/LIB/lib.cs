using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
	public class LIB
	{
		private LIB ()
		{
		}

		/// <summary>
		/// Extracts the name of the file.
		/// </summary>
		/// <returns>
		/// The filename only.
		/// </returns>
		/// <param name='fileName'>
		/// Filename with path.
		/// </param>
		public static String extractFileName(String fileName)
    	{
    		return (fileName.LastIndexOf('/')==0 ? fileName : fileName.Substring(fileName.LastIndexOf('/')+1));
    	}

		/// <summary>
		/// Reads the text from the server/client
		/// </summary>
		/// <returns>
		/// The text.
		/// </returns>
		/// <param name='io'>
		/// Network stream for reading from the server/client.
		/// </param>
		public static String readTextTCP (NetworkStream io)
		{
	        String line = "";
	        char ch;
	        
	        while((ch = (char)io.ReadByte()) != 0)
	        	line += ch;

	        return line;
		}

		/// <summary>
		/// Writes the text to the server/client
		/// </summary>
		/// <param name='outToServer'>
		/// Network stream for writing from the server/client.
		/// </param>
		/// <param name='line'>
		/// The text to write to the server/client
		/// </param>
		public static void writeTextTCP(NetworkStream outToServer, String line)
		{
			System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
			outToServer.Write(encoding.GetBytes(line), 0, line.Length);
			outToServer.WriteByte(0);
		}

		/// <summary>
		/// Gets the file size from the server.
		/// </summary>
		/// <returns>
		/// The filesize as a long.
		/// </returns>
		/// <param name='inFromServer'>
		/// Network stream for reading from the server.
		/// </param>
	    public static long getFileSizeTCP(NetworkStream inFromServer)
	    {
	    	return long.Parse(readTextTCP(inFromServer));
	    }

		/// <summary>
		/// Check_s the file_ exists.
		/// </summary>
		/// <returns>
		/// The filesize.
		/// </returns>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		public static long check_File_Exists (String fileName)
		{
			if (File.Exists (fileName))
				return (new FileInfo(fileName)).Length;

			return 0;
		}
	}
}

