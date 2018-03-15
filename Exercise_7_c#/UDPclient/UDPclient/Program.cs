using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace udp
{
	class UDPClient
	{
		const int PORT = 9000;


		private UDPClient()
		{
			var client = new UdpClient ();

			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("10.0.0.1", PORT);

			client.Connect(EndPoint, PORT);

			
				
			Byte[] receivedData = server.Receive (ref endPoint); //listen on port 9000

			string data = Encoding.ASCII.GetString (receivedData);

			Console.Write("receive data from: " + data);

			//				byte[] dataToSend = new byte[1] { 1 };
			//				//dataToSend = ;
			//				server.Send(dataToSend, dataToSend.Length, ep); //reply back
			}

		}

		static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new UDPClient ();
		}
	}
}
