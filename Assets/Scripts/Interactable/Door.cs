using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable{

    [Header("General Door Stats")]
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private int _keyID = 0;            //0 Means it doesn't require a key

    //Interacts with the object
    public override void Interact() {
        //Changes the Scene of the Game
        if (_isLocked) {
            //Checks the Inventory to see if the player has this key
            return;
        }
    }

    //Starts the Hovering
    public override void Hover() {
        //Shoots unlock message
        if (_isLocked) {
            UpdateToolTip("It appears to be locked.");
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