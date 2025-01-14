using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable{

    [Header("General Door Stats")]
    [SerializeField] private bool _autoUnlock = false;
    [SerializeField] private NPC _npc; 
    [SerializeField] private string _sceneName = "";
    [SerializeField] private string _lockedMessage = "";
    [SerializeField] public bool _isLocked = false;

    //Interacts with the object
    public override void Interact() {
        //Changes the Scene of the Game
        if (_isLocked) {
            //Checks the Inventory to see if the player has this key
            return;
        }

        //Not Locked
        GameManager.Manager.LevelManager.ChangeScene(_sceneName);
    }

    private void FixedUpdate() {
        //Exits if the door is already unlocked
        if (!_isLocked) return;

        //Checks for the NPC to be with their quest to open
        if (_autoUnlock && _npc._finishedQuest) _isLocked = false;
    }

    //Starts the Hovering
    public override void Hover() {
        //Shoots unlock message
        if (_isLocked) {
            UpdateToolTip(_lockedMessage);
            return;
        }

        //Has the key and can enter
        UpdateToolTip("Press E to Enter Door.");
    }

    //Ends the Hovering
    public override void EndHover() {
        UpdateToolTip();
    }
}
