using System.Net;
using FireWallEngine;

namespace FireWallDebugConsole;

public class DebugEvent : IFireWallEventHandler
{
    public void TcpConnectBlocked(BlockTypes type, IPEndPoint remoteAddress, IPEndPoint localAddress)
    {
        
    }

    public void UdpMessageBlocked(BlockTypes type, IPEndPoint remoteAddress, IPEndPoint localAddress)
    {
        
    }

    public void TcpMessageBlocked(BlockTypes type, IPEndPoint remoteAddress, IPEndPoint localAddress)
    {
        
    }

    public void TcpConnectAccepted(IPEndPoint remoteAddress, IPEndPoint localAddress)
    {
        
    }

    public void UdpMessageAccepted(IPEndPoint remoteAddress, IPEndPoint localAddress)
    {
        
    }

    public void TcpMessageAccepted(IPEndPoint input, IPEndPoint output)
    {
        Console.WriteLine(input.Address.ToString() +  " => " + output.Address.ToString());
    }
}