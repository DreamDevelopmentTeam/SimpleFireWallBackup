using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FireWallEngine
{
    public class ProxyEngine
    {
        private readonly IPAddress _localIP;
        private readonly int _localPort;
        private readonly IPAddress _remoteIP;
        private readonly int _remotePort;
        private readonly ProtocolType _protocolType;
        public List<string> IpBlackList = new List<string>();

        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort, ProtocolType protocolType)
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;

            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
        }

        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort,
            ProtocolType protocolType, List<string> ipBlackList)
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;
            this.IpBlackList = ipBlackList;

            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            
        }

        private void StartProxy()
        {
            if (_protocolType == ProtocolType.Tcp)
            {
                TcpListener listener = new TcpListener(_localIP, _localPort);
                listener.Start();

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    if (IpBlackList.Contains(clientIp))
                    {
                        client.Close();
                        continue;
                    }
                    
                    NetworkStream clientStream = client.GetStream();
                    
                    TcpClient server = new TcpClient();
                    server.Connect(_remoteIP, _remotePort);
                    NetworkStream serverStream = server.GetStream();

                    // TODO: Add your firewall logic here

                    Thread clientToServerThread = new Thread(() => Relay(clientStream, serverStream));
                    Thread serverToClientThread = new Thread(() => Relay(serverStream, clientStream));

                    clientToServerThread.Start();
                    serverToClientThread.Start();
                }
            }
            else if (_protocolType == ProtocolType.Udp)
            {
                UdpClient listener = new UdpClient(_localPort);

                while (true)
                {
                    IPEndPoint endPoint = new IPEndPoint(_localIP, _localPort);
                    string clientIp = endPoint.Address.ToString();
                    if (IpBlackList.Contains(clientIp))
                    {
                        continue;
                    }
                    byte[] bytes = listener.Receive(ref endPoint);

                    // TODO: Add your firewall logic here

                    UdpClient sender = new UdpClient();
                    sender.Connect(_remoteIP, _remotePort);
                    sender.Send(bytes, bytes.Length);
                }
            }
        }

        private void Relay(NetworkStream input, NetworkStream output)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;

            try
            {
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }
            }
            catch (Exception e)
            {
                // Handle the exception as needed
                Console.WriteLine(e.Message);
            }
        }
        
    }
}
