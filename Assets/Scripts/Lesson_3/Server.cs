using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;
    private int port = 5805;
    private int hostID;
    private int reliableChannel;
    private bool isStarted = false;
    private byte error; Dictionary<int, string> connectionIDs = new Dictionary<int, string>();

    private void Update()
    {
        if (!isStarted) return;
        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    connectionIDs.Add(connectionId, "");
                    break;
                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    SendSpecificMessage(connectionId, message, ParseMassage(connectionId, message));
                    break;
                case NetworkEventType.DisconnectEvent:
                    connectionIDs.Remove(connectionId);
                    SendMessageToAll($"Player{connectionId}has disconnected.");
                    Debug.Log($"Player{connectionId}has disconnected.");
                    break;
                    case NetworkEventType.BroadcastEvent:
                    break;
            }
            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        }
    }

    private void SendSpecificMessage(int connectionId, string message, MessageType messageType)
    {
        if (messageType == MessageType.name)
        {
            SendMessageToAll($"Player{connectionIDs[connectionId]} has connected");
            Debug.Log($"Player{connectionId}:{message}");
            return;
        }
        if (messageType == MessageType.message)
        {
            SendMessageToAll($"Player{connectionIDs[connectionId]}:{message}");
            Debug.Log($"Player{connectionIDs[connectionId]}:{message} : {connectionId}");
        }

    }

    private MessageType ParseMassage(int connectionId, string message)
    {
        string[] messages = message.Split("__");
        string aggregateMessage = AggregateMessage(messages);
        if (messages[0] == "name")
        {
            connectionIDs[connectionId] = aggregateMessage;
            if (message == string.Empty)
            {
                connectionIDs[connectionId] =  connectionId.ToString();
            }
            return MessageType.name;
        }
        if (messages[0] == "massage")
        {
            return MessageType.message;
        }
        return MessageType.nu1l;
    }

    private string AggregateMessage(string[] messages)
    {
        string message = messages[1];
        for(int i = 2; i < messages.Length; i++)
        {
            message += "__" + messages[i];
        }
        return message;
    }
    public void StartServer()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
        hostID = NetworkTransport.AddHost(topology, port);

        isStarted = true;
    }

    public void ShutDownServer()
    {
        if (!isStarted)
        {
            return;
        }

        NetworkTransport.RemoveHost(hostID);
        NetworkTransport.Shutdown();
        isStarted = false;
    }

    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log((NetworkError)error);
        }
    }

    public void SendMessageToAll(string message)
    {
        for (int i = 0; i < connectionIDs.Count; i++)
        {
            SendMessage(message, connectionIDs[i]);
        }

    }
}
