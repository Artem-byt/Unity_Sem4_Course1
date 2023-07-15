using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawns;
    private GameObject playerCharacter;
    private List<Transform> spawnTransforms;

    private void Start() 
    {
        spawnTransforms = new List<Transform>();
        spawnTransforms = spawns.GetComponentsInChildren<Transform>().ToList();
        SpawnCharacter(); 
    }

    public void SpawnCharacter() 
    {
        if (!isServer) 
        { 
            return; 
        } 

        var randomSpawn = Random.Range(0, spawnTransforms.Count - 1);
        playerCharacter = Instantiate(playerPrefab, spawnTransforms[randomSpawn].position, spawnTransforms[randomSpawn].rotation);
        
        NetworkServer.SpawnWithClientAuthority(playerCharacter, connectionToClient);
    }
}
