using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using KRPC.Client;

namespace PEGAS
{
	public class KSPConnection
	{
		public KSPConnection(string name, IPAddress address, int rpcPort, int streamPort)
		{
			Connection = new Connection(name, address, rpcPort, streamPort);
		}

		public Connection Connection { get; }
	}
}
