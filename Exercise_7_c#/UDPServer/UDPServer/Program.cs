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
				IPEndPoint ep = new IPEndPoint(IPAddress.Any, PORT);
				var data = server.Receive (ref ep); //listen on port 9000
				Console.Write("receive data from: " + ep.ToString());

				byte[] dataToSend = new byte[1] { 1 };
				//dataToSend = ;
				server.Send(dataToSend, dataToSend.Length, ep); //reply back
			}
		
		}
	
		static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new UDPServer ();
		}
	}
}
