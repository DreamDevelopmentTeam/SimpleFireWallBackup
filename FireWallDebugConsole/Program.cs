// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using FireWallDebugConsole;
using FireWallEngine;

Console.WriteLine("Hello, World!");

Dictionary<string, int> ipBlackList = new Dictionary<string, int>()
{
    { "127.0.0.1", 0 }
};
Logger loggerA = new Logger(true, true, "logA.txt");
Logger loggerB = new Logger(true, true, "logB.txt");

DebugEvent debugEvent = new DebugEvent();

ProxyEngine engineA = new ProxyEngine(
        IPAddress.Any, 
        8001,
        IPAddress.Parse("127.0.0.1"),
        8000,
        ProtocolType.Tcp,
        ipBlackList
    );
    
ProxyEngine engineB = new ProxyEngine(
    IPAddress.Any, 
    8002,
    IPAddress.Parse("127.0.0.1"),
    8000,
    ProtocolType.Tcp,
    new Dictionary<string, int>()
);
engineB.SetEventHandler(debugEvent);
engineA.SetLogger(loggerA, loggerName: "ProxyA");
engineB.SetLogger(loggerB, loggerName: "ProxyB");
