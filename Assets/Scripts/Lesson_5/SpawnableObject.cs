using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class SpawnableObject : NetworkBehaviour
{
    [SerializeField] private GameObject _spawnPrefab;

    protected void SpawnObjects(NetworkStartPosition[] positionsForRespawn)
    {
        for(int i=0; i< positionsForRespawn.Length; i++)
        {
            var object1 = Instantiate(_spawnPrefab, positionsForRespawn[i].transform.position, positionsForRespawn[i].transform.rotation);
            NetworkServer.Spawn(object1);
        }
    }
}
