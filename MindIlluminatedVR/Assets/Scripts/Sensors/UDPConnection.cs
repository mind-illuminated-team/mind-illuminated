using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System;
using System.Collections.Generic;

namespace Sensors
{
    public class UDPConnection
    {

        public static UDPConnection Instance { get; } = new UDPConnection();

        private readonly int portNumber = 5000;

        private readonly List<IUDPDataListener> listeners = new List<IUDPDataListener>();

        private Thread listeningThread;

        private UdpClient udp;

        private UDPConnection()
        {
            Debug.Log("Starting UDP listener on port " + portNumber);

            listeningThread = new Thread(new ThreadStart(UdpListener))
            {
                IsBackground = true
            };

            listeningThread.Start();
        }

        public void UdpListener()
        {
            udp = new UdpClient(portNumber);

            while (true)
            {
                //Listening 
                try
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, 5000);

                    byte[] receiveBytes = udp.Receive(ref RemoteIpEndPoint);

                    if (receiveBytes != null)
                    {
                        string data = Encoding.ASCII.GetString(receiveBytes);
                        Debug.Log("Message Received: " + data.ToString() +
                            "\nAddress IP Sender: " + RemoteIpEndPoint.Address.ToString() +
                            "\nPort Number Sender: " + RemoteIpEndPoint.Port.ToString());

                        listeners.ForEach(l => l.Listen(data));
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        }

        public void RegisterListener(IUDPDataListener listener)
        {
            listeners.Add(listener);
        }

        ~UDPConnection()
        {
            if (listeningThread != null && listeningThread.IsAlive)
            {
                listeningThread.Abort();
            }

            udp.Close();
        }

    }
}

