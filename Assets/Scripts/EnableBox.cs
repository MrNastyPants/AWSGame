using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPC _mover;

    private void FixedUpdate() {
        //Exits if you haven't passed the quest yet.
        if (!_mover._passedQuest) return;

        //If you have then enable the box and turns itself off.
        transform.Find("Mesh").GetComponent<MeshRenderer>().enabled = true;
        this.enabled = false;
    }
}
