using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commenter : Interactable
{
    [Header("Messages")]
    [SerializeField] private string _message;

    public override void Hover() {
        UpdateToolTip(_message);
    }
    public override void EndHover() {
        UpdateToolTip();
    }
}
