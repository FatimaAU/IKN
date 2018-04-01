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

		private UDPClient(string[] args)
		{

			// Validate parameter list
			if (args.Length != 2) 
			{
				Console.WriteLine ("Bad parameterlist. Please type hostname and valid input for reading (U)ptime and (L)oadavg");
				Environment.Exit (1);
			}

			// Create new UDPClient
			var client = new UdpClient ();

			// Connect to end point on host with PORT 9000
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(args[0]), PORT);
			client.Connect(endPoint);

			// Send reading request to server
			byte[] sentData = Encoding.ASCII.GetBytes (args[1]);
			client.Send (sentData, sentData.Length);

			// Receive data from server and print on screen
			byte[] receivedData = client.Receive (ref endPoint);
			string data = Encoding.ASCII.GetString (receivedData);
			Console.WriteLine (data);
		}

		static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");
			new UDPClient (args);
		}
	}
}

