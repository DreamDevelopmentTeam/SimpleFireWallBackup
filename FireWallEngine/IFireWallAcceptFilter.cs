using System.Net;

namespace FireWallEngine;

public interface IFireWallAcceptFilter
{
    public bool UDPFilter(IPEndPoint remote, IPEndPoint local);
    public bool TCPFilter(IPEndPoint remote, IPEndPoint local);
}