using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLabel : MonoBehaviour
{
    public void DrawLabel(Camera camera) 
    { 
        if (camera == null) 
        { 
            return;
        } 
        var style = new GUIStyle(); 
        style.normal.background = Texture2D.redTexture; 
        style.normal.textColor = Color.blue; 
        var objects = ClientScene.objects;
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects.ElementAt(i).Value;
            var position = camera.WorldToScreenPoint(obj.transform.position);
            var collider = obj.GetComponent<Collider>();

            string name;
            if (obj.GetComponent<ShipController>())
            {
                name = obj.GetComponent<ShipController>().playerName;
            }
            else
            {
                name = obj.name;
            }
            if (collider != null && isVisibleObjectbyCamera(camera, collider) && obj.transform != transform) 
            { 
                GUI.Label(new Rect(new Vector2(position.x, Screen.height - position.y), new Vector2(10, name.Length * 10.5f)), name, style);
            }
        }
    }

    private bool isVisibleObjectbyCamera(Camera camera, Collider collider)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(camera);
        var point = collider.transform.position;

        foreach(var plane in planes )
        {
            if(plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }
}
