using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MessagePlayerName : MessageBase
{
    public string name;

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(name);
    }

    public override void Deserialize(NetworkReader reader)
    {
       name = reader.ReadString();
    }
}
