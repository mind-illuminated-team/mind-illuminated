using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System;
using UnityEngine.UI;

public class UDPConnection : MonoBehaviour
{
    public int portNumber = 5000;

    
    public Text dataDisplay;

    private string sensorData;

    private Thread listeningThread;

    private UdpClient udp;

    // Use this for initialization
    void Start()
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
                    Debug.Log("Message Received" + data.ToString());
                    Debug.Log("Address IP Sender" + RemoteIpEndPoint.Address.ToString());
                    Debug.Log("Port Number Sender" + RemoteIpEndPoint.Port.ToString());

                    sensorData = data;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        dataDisplay.text = sensorData;
    }

    void OnDisable()
    {
        if (listeningThread != null && listeningThread.IsAlive)
        {
            listeningThread.Abort();
        }

        udp.Close();
    }
}
