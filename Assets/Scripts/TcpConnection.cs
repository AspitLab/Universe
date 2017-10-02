using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

public class TcpConnection : MonoBehaviour {

    [SerializeField]
    private string m_StartMessage = "Hello World";

    public string m_IPAdress = "127.0.0.1";
    public const int kPort = 8001;
    public int bytes;
    private Socket m_Socket;
    public byte[] bytesReceived;
    public string receivedData = "";


    void Awake()
    {
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(m_IPAdress);
        System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, kPort);
        m_Socket.Connect(remoteEndPoint);
        SendMessage(m_StartMessage);
    }

    // Update is called once per frame
    void Update () {
        bytes = m_Socket.Receive(bytesReceived, bytesReceived.Length, 0);
        while (bytes != 0)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            string receivedData = encoding.GetString(bytesReceived);
        }
    }
    void OnApplicationQuit()
    {
        m_Socket.Close();
        m_Socket = null;
    }

    void SendMessage(string message)
    {
        byte[] byData = System.Text.Encoding.ASCII.GetBytes(message);
        m_Socket.Send(byData);
    }
}
