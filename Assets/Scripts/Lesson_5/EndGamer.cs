using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EndGamer : MonoBehaviour
{
     [SerializeField] private GameObject _canvasPrefab;

    private CanvasComponents _canvas;

    private void Start()
    {
        _canvas = Instantiate(_canvasPrefab).GetComponentInChildren<CanvasComponents>();
    }

    public void EndGame(string[] playersName, int[] playersPoints)
    {
        _canvas.gameObject.SetActive(true);

        var ships = new List<Player>();
        for (int i = 0; i < playersName.Length; i++)
        {
            ships.Add(new Player(playersName[i], playersPoints[i]));
        }

        ships.OrderByDescending(x => x.NumberOfPoints);

        var texts = _canvas.testFields;

        for (int i = 0; i < ships.Count; i++)
        {
            if (i > 3)
            {
                break;
            }

            texts[i].text= ships[i].PlayerName + " : " + ships[i].NumberOfPoints;
        }
        
    }

    public struct Player
    {
        public string PlayerName;
        public int NumberOfPoints;

        public Player(string name, int point)
        {
            PlayerName = name;
            NumberOfPoints = point;
        }
    }
}
