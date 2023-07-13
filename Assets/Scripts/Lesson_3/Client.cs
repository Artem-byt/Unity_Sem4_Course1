using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using UnityEditor.VersionControl;

public class Client : MonoBehaviour
{
    public delegate void OnMessageReceive(object message);
    public event OnMessageReceive onMessageReceive;

    private const int MAX_CONNECTION = 10;

    private int _port = 0;
    private int _serverPort = 5805;
    private int _hostID;
    private int _reliableChannel;
    private int _connectionID;
    private bool _isConnected = false;
    private byte _error;
    private string _name;

    public void Connect()
    {
        NetworkTransport.Init(); 
        ConnectionConfig cc = new ConnectionConfig(); 
        _reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
        _hostID = NetworkTransport.AddHost(topology, _port);

        _connectionID = NetworkTransport.Connect(_hostID, "127.0.0.1", _serverPort, 0, out _error);

        if ((NetworkError)_error == NetworkError.Ok)
        {
            _isConnected = true;
        }
        else
        {
            Debug.Log((NetworkError)_error);

        }

    }
    public void SetName(string name)
    {
        _name = name;
    }

    public void Disconnect()
    {
        if (!_isConnected) return; 
        NetworkTransport.Disconnect(_hostID, _connectionID, out _error); 
        _isConnected = false;
    }

    private void Update()
    {
        if (!_isConnected)
        {
            return;
        }
        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out _error);
        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    onMessageReceive?.Invoke($"Youhave been connected to server."); 
                    Debug.Log($"You have been connectedto server.");
                    break;
                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize); 
                    onMessageReceive?.Invoke(message); 
                    Debug.Log(message);
                    break;
                case NetworkEventType.DisconnectEvent:
                    _isConnected = false; 
                    onMessageReceive?.Invoke($"Youhave been disconnected from server."); 
                    Debug.Log($"You have been disconnectedfrom server.");
                    break;
                case NetworkEventType.BroadcastEvent:
                    break;
            }
            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out _error);
        }
    }

    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(_hostID, connectionID, _reliableChannel, buffer, message.Length * sizeof(char), out _error);
        if ((NetworkError)_error != NetworkError.Ok)
        {
            Debug.Log((NetworkError)_error);
        }
    }
}
