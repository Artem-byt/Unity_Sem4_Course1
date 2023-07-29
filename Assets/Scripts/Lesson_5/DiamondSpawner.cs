using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DiamondSpawner : SpawnableObject
{
    [SerializeField] private NetworkStartPosition[] _spawns;

    private void Start()
    {
        if(isServer)
        {
            SpawnObjects(_spawns);
        }
        
    }
}
