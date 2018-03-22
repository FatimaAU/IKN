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


				GetMeasurement (data);
				Console.WriteLine (data);
				byte[] sentdataback = Encoding.ASCII.GetBytes (data);
				server.Send (sentdataback, sentdataback.Length);					


				Console.WriteLine("receive data from: " + data);

//				byte[] dataToSend = new byte[1] { 1 };
//				//dataToSend = ;
//				server.Send(dataToSend, dataToSend.Length, ep); //reply back
			}
		
		}

		public string GetMeasurement(string letter)
		{
//			string uptime = System.IO.File.ReadAllText ("root/proc/uptime");
//			string loadavg = System.IO.File.ReadAllText ("/~/proc/loadavg");

			if (letter == "u") 
			{
				return "derp";
			}
			if (letter == "U")
			{
				return "herp";
			}
			else
				return "not the right letter";

		}
	
		static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new UDPServer ();
		}
	}
}
