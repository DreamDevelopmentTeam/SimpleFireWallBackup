using System.Net;

namespace FireWallEngine;

public interface IFireWallEventHandler
{
    public void TcpConnectBlocked(BlockTypes type, IPEndPoint remoteAddress, IPEndPoint localAddress);
    public void UdpMessageBlocked(BlockTypes type, IPEndPoint remoteAddress, IPEndPoint localAddress);
    public void TcpMessageBlocked(BlockTypes type, IPEndPoint remoteAddress, IPEndPoint localAddress);
    
    public void TcpConnectAccepted(IPEndPoint remoteAddress, IPEndPoint localAddress);
    public void UdpMessageAccepted(IPEndPoint remoteAddress, IPEndPoint localAddress);
    public void TcpMessageAccepted(IPEndPoint input, IPEndPoint output);
}