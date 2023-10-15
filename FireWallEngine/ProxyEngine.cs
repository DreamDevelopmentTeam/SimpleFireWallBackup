using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
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
        public Dictionary<string, int> IpBlackList = new Dictionary<string, int>();
        public Dictionary<string, int> IpWhiteList = new Dictionary<string, int>();
        public Dictionary<Regex, int> IpBlackListRegex = new Dictionary<Regex, int>();
        public Dictionary<Regex, int> IpWhiteListRegex = new Dictionary<Regex, int>();

        private IFireWallTrafficFilter trafficFilter = null;
        private IFireWallAcceptFilter acceptFilter = null;
        private IFireWallEventHandler eventHandler = null;

        private Logger logger;
        private string loggerName = "Proxy";
        

        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort, ProtocolType protocolType)
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;

            /*
            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            */
        }

        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort,
            ProtocolType protocolType, Dictionary<string, int> ipBlackList)
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;
            this.IpBlackList = ipBlackList;

            /*
            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            */
            
        }
        
        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort,
            ProtocolType protocolType, Dictionary<string, int> ipBlackList, Dictionary<string, int> ipWhiteList)
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;
            this.IpBlackList = ipBlackList;
            this.IpWhiteList = IpWhiteList;

            /*
            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            */
            
        }
        
        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort,
            ProtocolType protocolType, Dictionary<Regex, int> ipBlackListRegex)
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;
            this.IpBlackListRegex = ipBlackListRegex;

            /*
            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            */
            
        }
        
        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort,
            ProtocolType protocolType, Dictionary<Regex, int> ipBlackListRegex, Dictionary<Regex, int> ipWhiteListRegex)
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;
            this.IpBlackListRegex = ipBlackListRegex;
            this.IpWhiteListRegex = IpWhiteListRegex;

            /*
            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            */
            
        }
        
        
        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort,
            ProtocolType protocolType, Dictionary<string, int> ipBlackList, Dictionary<Regex, int> ipBlackListRegex )
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;
            this.IpBlackList = ipBlackList;
            this.IpBlackListRegex = ipBlackListRegex;

            /*
            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            */
            
        }
        
        public ProxyEngine(IPAddress localIP, int localPort, IPAddress remoteIP, int remotePort,
            ProtocolType protocolType, Dictionary<string, int> ipBlackList, Dictionary<string, int> ipWhiteList
            , Dictionary<Regex, int> ipBlackListRegex, Dictionary<Regex, int> ipWhiteListRegex)
        {
            this._localIP = localIP;
            this._localPort = localPort;
            this._remoteIP = remoteIP;
            this._remotePort = remotePort;
            this._protocolType = protocolType;
            this.IpBlackList = ipBlackList;
            this.IpWhiteList = IpWhiteList;
            this.IpBlackListRegex = ipBlackListRegex;
            this.IpWhiteListRegex = ipWhiteListRegex;

            /*
            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            */
        }

        public Thread Start()
        {
            Thread proxyThread = new Thread(StartProxy);
            proxyThread.Start();
            return proxyThread;
        }


        public void SetLogger(Logger logger , string loggerName = "Proxy")
        {
            this.loggerName = loggerName;
            this.logger = logger;
        }

        public void SetTrafficFilter(IFireWallTrafficFilter trafficFilter)
        {
            this.trafficFilter = trafficFilter;
        }
        
        public void SetAcceptFilter(IFireWallAcceptFilter acceptFilter)
        {
            this.acceptFilter = acceptFilter;
        }

        public void SetEventHandler(IFireWallEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
        }


        private void StartProxy()
        {
            logger.Info(this.loggerName, $"Starting firewall proxy on {_localIP}:{_localPort} to {_remoteIP}:{_remotePort}");
            if (_protocolType == ProtocolType.Tcp)
            {
                TcpListener listener = new TcpListener(_localIP, _localPort);
                listener.Start();

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    int port = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
                    if (IpBlackList.ContainsKey(clientIp))
                    {
                        if (IpBlackList[clientIp] == 0 || IpBlackList[clientIp] == port)
                        {
                            logger.Warn(this.loggerName, $"Blocked TCP connection from {clientIp}:{port} to {_remoteIP}:{_remotePort}");
                            if (this.eventHandler != null)
                            {
                                this.eventHandler.TcpConnectBlocked(BlockTypes.IpBlackList, ((IPEndPoint)client.Client.RemoteEndPoint), new IPEndPoint(this._localIP, this._localPort));
                            }

                            client.Close();
                            continue;
                        }
                    }

                    if (this.acceptFilter != null)
                    {
                        if (!this.acceptFilter.TCPFilter(((IPEndPoint)client.Client.RemoteEndPoint), new IPEndPoint(this._localIP, this._localPort)))
                        {
                            logger.Warn(this.loggerName, $"(Accept Filter) Rejected TCP connection from {clientIp}:{port} to {_remoteIP}:{_remotePort}");
                            if (this.eventHandler != null)
                            {
                                this.eventHandler.TcpConnectBlocked(BlockTypes.InterFace, ((IPEndPoint)client.Client.RemoteEndPoint), new IPEndPoint(this._localIP, this._localPort));
                            }
                            
                            client.Close();                            
                            continue;
                        }
                    }
                    
                    foreach(Regex regex in IpBlackListRegex.Keys)
                    {
                        if (regex.IsMatch(clientIp))
                        {
                            if (IpBlackListRegex[regex] == 0 || IpBlackListRegex[regex] == port)
                            {
                                logger.Warn(this.loggerName,
                                    $"(Regex) Rejected TCP connection from {clientIp}:{port} to {_remoteIP}:{_remotePort}");
                                if (this.eventHandler != null)
                                {
                                    this.eventHandler.TcpConnectBlocked(BlockTypes.IpBlackListRegex, ((IPEndPoint)client.Client.RemoteEndPoint), new IPEndPoint(this._localIP, this._localPort));
                                }
                                client.Close();                                
                                continue;
                            }
                            
                        }

                    }

                    logger.Info(this.loggerName, $"Accepted TCP connection from {clientIp}:{port} to {_remoteIP}:{_remotePort}");
                    if (this.eventHandler != null)
                    {
                        this.eventHandler.TcpConnectAccepted((IPEndPoint)client.Client.RemoteEndPoint , new IPEndPoint(this._localIP, this._localPort));
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
                    byte[] bytes = listener.Receive(ref endPoint);
                    string clientIp = endPoint.Address.ToString();
                    int port = endPoint.Port;
                    if (IpBlackList.ContainsKey(clientIp))
                    {
                        if (IpBlackList[clientIp] == 0 || IpBlackList[clientIp] == port)
                        {
                            logger.Warn(this.loggerName,$"Blocked UDP message from {clientIp}:{port} to {_remoteIP}:{_remotePort}");
                            if (this.eventHandler != null)
                            {
                                this.eventHandler.UdpMessageBlocked(BlockTypes.IpBlackList, endPoint, new IPEndPoint(this._localIP, this._localPort));
                            }
                            continue;
                        }
                    }
                    if (this.acceptFilter != null)
                    {
                        if (!this.acceptFilter.UDPFilter(endPoint, new IPEndPoint(this._localIP, this._localPort)))
                        {
                            logger.Warn(this.loggerName, $"(Accept Filter) Rejected UDP message from {clientIp}:{port} to {_remoteIP}:{_remotePort}");
                            if (this.eventHandler != null)
                            {
                                this.eventHandler.UdpMessageBlocked(BlockTypes.InterFace, endPoint, new IPEndPoint(this._localIP, this._localPort));
                            }
                            continue;
                        }
                    }
                    
                    foreach(Regex regex in IpBlackListRegex.Keys)
                    {
                        if (regex.IsMatch(clientIp))
                        {
                            if (IpBlackListRegex[regex] == 0 || IpBlackListRegex[regex] == port)
                            {
                                logger.Warn(this.loggerName,
                                    $"(Regex) Rejected UDP message from {clientIp}:{port} to {_remoteIP}:{_remotePort}");
                                if (this.eventHandler != null)
                                {
                                    this.eventHandler.UdpMessageBlocked(BlockTypes.IpBlackListRegex, endPoint, new IPEndPoint(this._localIP, this._localPort));
                                }
                                continue;
                            }
                            
                        }

                    }
                    
                    // TODO: Add your firewall logic here
                    
                    if (this.trafficFilter != null)
                    {
                        if (!this.trafficFilter.UDPTrafficFilter(endPoint, bytes, bytes.Length))
                        {
                            logger.Warn(this.loggerName, $"(Traffic Filter) Rejected UDP message from {clientIp}:{port} to {_remoteIP}:{_remotePort}");       
                            if (this.eventHandler != null)
                            {
                                this.eventHandler.UdpMessageBlocked(BlockTypes.TrafficInterFace, endPoint, new IPEndPoint(this._localIP, this._localPort));
                            }
                            continue;
                        }
                    }

                    logger.Info(this.loggerName,$"Accepted UDP message from {clientIp}:{port} to {_remoteIP}:{_remotePort}");
                    
                    if (this.eventHandler != null)
                    {
                        this.eventHandler.UdpMessageAccepted(endPoint, new IPEndPoint(this._localIP, this._localPort));
                    }
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

            Socket inputSocket = input.Socket;
            IPEndPoint inputEndPoint = (IPEndPoint)inputSocket.RemoteEndPoint;
            AddressFamily inputAddressFamily = inputEndPoint.AddressFamily;
            int inputPort = inputEndPoint.Port;
            
            Socket outputSocket = output.Socket;
            IPEndPoint outputEndPoint = (IPEndPoint)inputSocket.RemoteEndPoint;
            AddressFamily outputAddressFamily = inputEndPoint.AddressFamily;
            int outputPort = inputEndPoint.Port;
            
            
            try
            {
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (this.trafficFilter != null)
                    {
                        if (!this.trafficFilter.TCPTrafficFilter(input, output, buffer, bytesRead))
                        {
                            logger.Warn(this.loggerName, $"(Traffic Filter) Rejected TCP message from {inputAddressFamily}:{inputPort} to {outputAddressFamily}:{outputPort}");                         
                            if (this.eventHandler != null)
                            {
                                this.eventHandler.TcpMessageBlocked(BlockTypes.TrafficInterFace, inputEndPoint, outputEndPoint);
                            }
                            continue;
                        }
                    }
                    if (this.eventHandler != null)
                    {
                        this.eventHandler.TcpMessageAccepted(inputEndPoint, outputEndPoint);
                    }
                    output.Write(buffer, 0, bytesRead);
                }
            }
            catch (Exception e)
            {
                // Handle the exception as needed
                //Console.WriteLine(e.Message);
                this.logger.Error(this.loggerName, $"Error while relaying data: {e.Message}", e);
            }
        }
        
        
        
    }
}
