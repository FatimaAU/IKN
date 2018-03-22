using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace udp
{
	class UDPServer
	{
		const int PORT = 9000;

	
		private UDPServer()
		{
			var server = new UdpClient (PORT);

			while (true) 
			{
				IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
				byte[] receivedData = server.Receive (ref endPoint); //listen on port 9000

				string data = Encoding.ASCII.GetString (receivedData);

				if (data == "U") 
				{
											
				}

				Console.Write("receive data from: " + data);

//				byte[] dataToSend = new byte[1] { 1 };
//				//dataToSend = ;
//				server.Send(dataToSend, dataToSend.Length, ep); //reply back
			}
		
		}
	
		static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new UDPServer ();
		}
	}
}
