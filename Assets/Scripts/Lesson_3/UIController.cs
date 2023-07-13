using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.VersionControl;
using UnityEditor.PackageManager;


public class UIController : MonoBehaviour
{
    [SerializeField] private Button buttonStartServer;
    [SerializeField] private Button buttonShutDownServer;
    [SerializeField] private Button buttonConnectClient;

    [SerializeField] private Button buttonDisconnectClient;
    [SerializeField] private Button buttonSendMessage;

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private TextField textField;
    [SerializeField] private Server server;
    [SerializeField] private Client client;


    private void Start()
    {
        buttonStartServer.onClick.AddListener(() => StartServer());
        buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
        buttonConnectClient.onClick.AddListener(() => Connect());
        buttonDisconnectClient.onClick.AddListener(() => Disconnect());
        buttonSendMessage.onClick.AddListener(() => SendMessage());
        client.onMessageReceive += ReceiveMessage;
    }
    private void StartServer() 
    {
        server.StartServer(); 
    }
    private void ShutDownServer()
    {
        server.ShutDownServer();
    }

    private void Connect()
    { 
        client.Connect();
        client.SendMessage("name__" + inputName.text);
        inputField.DeactivateInputField();
    }

    private void Disconnect() 
    {
        client.Disconnect();
        inputField.ActivateInputField();
        inputName.text = "";
    }
    private void SendMessage() {
        client.SendMessage("massage__" + inputField.text); 
        inputField.text = ""; 
    }
    public void ReceiveMessage(object message) 
    {
        textField.ReceiveMessage(message); 
    }
}
