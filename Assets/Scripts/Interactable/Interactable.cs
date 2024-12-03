using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("General Stats")]
    [SerializeField] protected string itemName;

    public virtual void Interact() {
        print("Interacted with the item.");
;    }
    public virtual void Hover() {
        print("Hovering over item.");
    }
    public virtual void EndHover() {
        print("Ended the Hover.");
    }

    //Sends the UI ToolTip a Message to be displayed.
    protected void UpdateToolTip(string text = "") {
        if (GameManager.Manager) {
            GameManager.Manager.HUD.UpdateToolTip(text);
        }
    }
}
