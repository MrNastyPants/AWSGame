using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue{

    //Important Information
    public string Name;
    public string Text;

    //Options. Reference to the desired option in the list.
    public string Response1 = "";
    public int Option1 = 0;

    public string Response2 = "";
    public int Option2 = 0;

    public string Response3 = "";
    public int Option3 = 0; 

    //Alt Options
    public bool isEnd = false;
    public bool AIResponse = false;
    public float giveMoney = 0;

    //Meta Data
    public List<Vector2Int> Prior = new List<Vector2Int>();
    public List<Vector2Int> Next = new List<Vector2Int>();

}
