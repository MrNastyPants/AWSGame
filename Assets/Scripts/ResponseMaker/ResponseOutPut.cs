using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ResponseData", menuName = "ScriptableObjects/ResponseData", order = 1)]
public class ResponseOutPut : ScriptableObject {
    public List<Dialogue> OutPutResponse = new List<Dialogue>();
    public List<Vector2> Positions = new List<Vector2>();
}

