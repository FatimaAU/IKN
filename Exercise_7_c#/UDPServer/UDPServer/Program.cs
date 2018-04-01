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

			// Establish end point
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);

			while (true) 
			{

				// Receive client input
				byte[] receivedData = server.Receive (ref endPoint); //listen on port 9000
				string data = Encoding.ASCII.GetString (receivedData);

				Console.WriteLine ("Received client input: " + data);

				// Get the requested info
				string measure = GetMeasurement (data);

				// Send data back
				byte[] sentdataback = Encoding.ASCII.GetBytes (measure);
				server.Send (sentdataback, sentdataback.Length, endPoint);					


			}
		
		}

		public string GetMeasurement(string letter)
		{
			
			string filePath;

			switch (letter = letter.ToUpper()) 
			{
				case "U":
					filePath = "/proc/uptime";
					Console.WriteLine ("Sending uptime");
					break;
				case "L":
					filePath = "/proc/loadavg";
					Console.WriteLine ("Sending loadavg");
					break;
				default:
					Console.WriteLine ("Bad input. Sending error message");
					return "Bad input. Valid inputs are l (L) or u (U)\n";
			

			}
			return "Reading from " + filePath + ": " + File.ReadAllText (filePath);
		}
	
		static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new UDPServer ();
		}
	}
}
