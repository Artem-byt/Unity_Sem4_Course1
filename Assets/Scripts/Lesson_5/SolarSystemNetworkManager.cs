using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SolarSystemNetworkManager : NetworkManager
{
    [SerializeField] private string _playerName;

    private void Awake()
    {
        _playerName = "Player Name";
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) 
    { 
        var spawnTransform = GetStartPosition(); 
        var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation); 
        player.GetComponent<ShipController>().PlayerName = _playerName; 
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId); 
    }

    private void OnGUI()
    {
        _playerName = GUI.TextField(new Rect(10, 10, 200, 20), _playerName);
    }
}
