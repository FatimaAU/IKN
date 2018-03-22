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
		const int BUFSIZE = 1000;


		private UDPClient()
		{
			var client = new UdpClient (PORT);

			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("10.0.0.1"), PORT);

			client.Connect(endPoint);

			while (true) 
			{
				
//				IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("10.0.0.1"), PORT);
//
//				client.Connect(endPoint);

				string test = Console.ReadLine();

				byte[] sentData = Encoding.ASCII.GetBytes (test);

				//string data = Encoding.ASCII.GetString (sentData);

				client.Send (sentData, sentData.Length);

				//Console.Write("sent data: " + test);
				//System.Threading.Thread.Sleep (1000);
			}

			//				byte[] dataToSend = new byte[1] { 1 };
			//				//dataToSend = ;
			//				server.Send(dataToSend, dataToSend.Length, ep); //reply back
		}

		static void Main (string[] args)
		{
			Console.WriteLine ("Client...");
			new UDPClient ();
		}
	}
}

