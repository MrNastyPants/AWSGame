using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable{

    [Header("General NPC Stats")]
    [SerializeField] private string NPCName;

    [Header("References")]
    [SerializeField] private GameObject _alertBox;
    [SerializeField] public ResponseOutPut response;

    //Properties
    protected GameObject AlertBox {
        get => _alertBox != null ? _alertBox : _alertBox = transform.Find("Alert").gameObject;
    }

    //Functions
    private void Start() {
        //Initializes the NPC
        AlertBox.SetActive(false);
    }

    public override void Interact() {
        //Opens up the dialogue box
        GameManager.Manager.StartDialogue(response);
    }
    public override void Hover() {
        AlertBox.SetActive(true);
    }
    public override void EndHover() {
        AlertBox.SetActive(false);
    }
}
