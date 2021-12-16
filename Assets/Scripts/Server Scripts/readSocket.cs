using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System;
using UnityEngine;

public class readSocket : MonoBehaviour
{

    //server initialization
    TcpListener listener;
    String msg1;
    String msg2 = "no u";

    // Start is called before the first frame update
    void Start()
    {

        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 55001);
        listener.Start();
        print("is listening.");
    }

    // Update is called once per frame
    void Update()
    {
        if (!listener.Pending())
        {
        }
        else
        {
            print("socket comes!");
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            StreamReader reader = new StreamReader(ns);
            msg1 = reader.ReadToEnd();
            print(msg1);


        }
    }
}
