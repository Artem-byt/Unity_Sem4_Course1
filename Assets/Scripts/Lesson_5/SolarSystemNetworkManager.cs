using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SolarSystemNetworkManager : NetworkManager
{
    [SerializeField] private string _playerName;

    private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();
    private void Awake()
    {
        _playerName = "Player Name";
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) 
    {
        var spawnTransform = GetStartPosition(); 
        var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation); 
        _players.Add(conn.connectionId, player.GetComponent<ShipController>());

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId); 
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler(100, ReceiveName);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        MessagePlayerName name = new MessagePlayerName();
        name.name = _playerName;
        conn.Send(100, name);
    }

    public void ReceiveName(NetworkMessage networkMessage) 
    {
        _players[networkMessage.conn.connectionId].playerName = networkMessage.reader.ReadString();
        _players[networkMessage.conn.connectionId].gameObject.name = _players[networkMessage.conn.connectionId].playerName;
    }


    private void OnGUI()
    {
        if (!IsClientConnected() && !NetworkServer.active && matchMaker == null)
        {
            _playerName = GUI.TextField(new Rect(10, 150, 200, 20), _playerName);
        }
    }

}
