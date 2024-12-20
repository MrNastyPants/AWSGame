using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHider : MonoBehaviour
{
    [Header("Wall Hider Settings")]
    [SerializeField] private List<WallHider> _otherWalls = new List<WallHider>();

    public void HideWalls(bool hide) {
        //Hides the walls
        for (int i = 0; i < transform.childCount; i++) {
            //Hides the props that are blocking the screen
            if (transform.GetChild(i).tag == "Prop") {
                transform.GetChild(i).gameObject.SetActive(!hide);
                continue;
            }

            //Checks to see if it has a box collider first
            if (transform.GetChild(i).GetComponent<MeshRenderer>())
                transform.GetChild(i).GetComponent<MeshRenderer>().enabled = !hide;
        }

        //Hides the other called walls
        for (int i = 0; i < _otherWalls.Count; i++)
            _otherWalls[i].HideWalls(hide);
    }
}
