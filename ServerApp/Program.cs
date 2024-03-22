﻿using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    public class ChatServer
    {
        const short port = 4040;
        const string JOIN_CMD = "$<join>";
        List<IPEndPoint> members ;
        UdpClient server ;
        IPEndPoint clientEndPoint = null;
        public ChatServer()
        {
            members = new List<IPEndPoint>();
            server = new UdpClient(port);
        }
        private void AddMember(IPEndPoint clientEndPoint)
        {
            members.Add(clientEndPoint);
            Console.WriteLine("Member was added !");
        }
        private void SendToAll(byte[] data)
        {
            foreach (var member in members)
            {
                server.SendAsync(data, data.Length, member);
            }
        }
        public void Start()
        {           
            while (true)
            {
                byte[] data = server.Receive(ref clientEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine($" Got message {message}  from : {clientEndPoint} " +
                    $"at {DateTime.Now.ToShortTimeString()}");
                switch (message)
                {
                    case JOIN_CMD:
                        AddMember(clientEndPoint);
                        break;
                    default:
                        SendToAll(data);
                        break;
                }
            }

        }
    }
    internal class Program
    {       
        static void Main(string[] args)
        {
            ChatServer server = new ChatServer();   
            server.Start();
              
            
        }
       
    }
}
