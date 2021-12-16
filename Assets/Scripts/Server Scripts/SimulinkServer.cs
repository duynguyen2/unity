using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Net;
using System.IO;
using static Controller;

public class SimulinkServer : MonoBehaviour
{
    //General Init
    private List<ServerClient> clients;
    private List<int> disconnectIndex;

    //Camera Variables
    public GameObject VRHand;
    private Transform handTransform;
    private Quaternion handRotation;

    public GameObject[] phalanxes = new GameObject[3];
    public Transform[] phalanxTransforms = new Transform[3];
    public float[] dataPosition;

    // main GUI components that appear for the user.
    [Header("Server Settings")]
    public int port = 55001;
    private TcpListener server;
    private bool serverStarted;

    [Header("Game Object Settings")]
    public string handName = "";

    StreamWriter writer;

    public class ServerClient
    {
        public TcpClient tcp;
        public string clientName;
        public List<GameObject> clientObj;

        public ServerClient(TcpClient clientSocket, string Name)
        {
            clientName = Name;
            tcp = clientSocket;
            clientObj = new List<GameObject>();
        }
    }

    void Awake()
    {
        VRHand = GameObject.Find("VRHand");
        handTransform = VRHand.transform;
        handName = VRHand.name;
        handRotation = handTransform.localRotation;
        phalanxes[0] = GameObject.Find("indexProximal");
        phalanxes[1] = GameObject.Find("indexMiddle");
        phalanxes[2] = GameObject.Find("indexDistal");

        for (int i = 0; i < phalanxes.Length; i++)
        {
            phalanxTransforms[i] = phalanxes[i].transform;
        }

        dataPosition = new float[4];
    }

    // Use this for initialization
    void Start()
    {

        clients = new List<ServerClient>();
        disconnectIndex = new List<int>();
        float[] temp = putTogether(isHolding(), getRotationX());
        for (int i = 1; i < dataPosition.Length; i++)
        {
            dataPosition[i] = temp[i];
        }

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            Startlistening();
            serverStarted = true;
            Debug.Log("Server has started on port: " + port.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Socket Error " + e.Message);
        }

        InvokeRepeating("UpdateLoop", 0f, 0.003f);
    }

    private void UpdateLoop()
    {
        if (this.VRHand == null)
            this.VRHand = GameObject.FindGameObjectWithTag("Controller");

        if (!serverStarted)
            return;
        if (clients.Count == 0)
            return;

        for (int c = 0; c < clients.Count; c++)
        {
            //Check if clients are connected
            if (!isConnected(clients[c].tcp))
            {
                clients[c].tcp.Close();
                disconnectIndex.Add(c);
                Debug.Log(clients[c].clientName + " has disconnected from the server");
                continue;
            }
            else
            {
                StartCoroutine(sendRotationX(clients[c]));
            }
        }

        //Clean up Disconnected Clients
        for (int i = 0; i < disconnectIndex.Count; i++)
        {
            clients.RemoveAt(disconnectIndex[i]);
        }
        disconnectIndex.Clear();
    }

    private byte[] ConvertFloat2Bytes(float[] floatArray)
    {
        var byteArray = new byte[floatArray.Length * sizeof(float)];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    private byte floatToByte(float f)
    {
        return (byte)(f * 255);
    }

    private float[] ConvertBytes2Float(byte[] byteArray)
    {
        var floatArray = new float[byteArray.Length / sizeof(float)];
        Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
        return floatArray;
    }

    //Checks if client is connected
    private bool isConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            { //Makes sure the client is connected
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {         //Polls the Client for activity
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0); //Checks for response
                }
                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    //Begins connection with client
    private void AcceptServerClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        ServerClient NewClient = new ServerClient(listener.EndAcceptTcpClient(ar), null);
        Debug.Log("Someone has connected");
        clients.Add(NewClient);
        Startlistening();
    }

    //Starts listening on server socket
    private void Startlistening()
    {
        server.BeginAcceptTcpClient(AcceptServerClient, server);
    }

    //Try to close all the connections gracefully
    void OnApplicationQuit()
    {
        for (int i = 0; i < clients.Count; i++)
        {
            try
            {
                clients[i].tcp.GetStream().Close();
                clients[i].tcp.Close();
            }
            catch { }
        }
        Debug.Log("Connections Closed");
    }

    //Sends out data
    public void sendData(ServerClient c, float[] data)
    {
        NetworkStream clientStream = c.tcp.GetStream();

        try
        {

            byte[] dataByte = ConvertFloat2Bytes(data);
            Debug.Log(dataByte.Length);
            clientStream.Write(dataByte, 0, dataByte.Length);
            Debug.Log("Data sent: " + dataByte[7] + " to " + c.clientName);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not write to client.\n Error:" + e);
        }
    }

    //Organizes and Sends Picture
    IEnumerator sendRotationX(ServerClient c)
    {
        while (false)
        {
            yield return null;
        }
        //float[] temp = putTogether(isHolding(), getRotationX());
        //for (int i = 0; i < dataPosition.Length; i++)
        //{
        //    dataPosition[i] = temp[i];
        //}
        dataPosition = new float[] { isHolding(), Controller.grip };
        sendData(c, dataPosition);
        Debug.Log("Captured Rotation");
    }

    public Quaternion returnRotation()
    {
        return handRotation;
    }

    public float[] putTogether(float t, float[] r)
    {

        float[] n = new float[r.Length + 1];
        n[0] = t;
        for (int i = 1; i < n.Length; i++)
        {
            n[i] = r[i - 1];
            //if (i == 2 || i == 3)
            //    n[i] += 90;
        }

        return n;
    }

    public float isHolding()
    {
        bool t = Controller.handHolding;
        if (t.Equals(true))
        { 
            return 1f;
        }
        else
        {
            return 0f;
        }
    }

    public float[] getRotationX()
    {
        float[] temp = new float[3];
        for (int i = 0; i < temp.Length; i++)
        {
            //temp[i] = UnityEditor.TransformUtils.GetInspectorRotation(phalanxTransforms[i]).x;
            temp[i] = Controller.returnGrip();
        }

        return temp;
    }

}
