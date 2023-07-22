using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
        client.SetName(inputName.text);
        client.Connect();
        inputName.interactable= false;
    }

    private void Disconnect() 
    {
        client.Disconnect();
        inputName.interactable = true;
        inputName.text = "";
    }
    private void SendMessage() {
        client.SendMessage("massage_" + inputField.text); 
        inputField.text = ""; 
    }
    public void ReceiveMessage(object message) 
    {
        textField.ReceiveMessage(message); 
    }
}
