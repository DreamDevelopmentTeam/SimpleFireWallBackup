using System.Net;
using System.Net.Sockets;

namespace FireWallEngine;

public interface IFireWallTrafficFilter
{

    public bool UDPTrafficFilter(IPEndPoint remoteEndPoint, byte[] data, int length);
    public bool TCPTrafficFilter(NetworkStream input, NetworkStream output, byte[] data, int length);

}