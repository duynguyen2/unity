using System.Collections;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class setupSocket : MonoBehaviour
{

    internal Boolean socketReady = false;

    // initialization
    TcpClient mySocket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;
    String host = "localhost";
    Int32 port = 55000;

    // Start is called before the first frame update
    void Start()
    {
        setUpSocket();
        Debug.Log("socket is set up");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUpSocket() {
        try
        {
            mySocket = new TcpClient(host, port);
            stream = mySocket.GetStream();
            writer = new StreamWriter(stream);
            socketReady = true;
            Byte[] sentBytes = Encoding.UTF8.GetBytes("works bozo!!!!");
            mySocket.GetStream().Write(sentBytes, 0, sentBytes.Length);
            Debug.Log("socket successfully sent");
        }
        catch (Exception e) {
            Debug.Log("Socket Error: " + e);
        }
    }
}
