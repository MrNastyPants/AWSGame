using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text _toolTip;
    [SerializeField] private List<GameObject> _menues;
    [SerializeField] private DialogueManager _chatManager;

    //Properties
    private Text ToolTip {
        get => _toolTip != null ? _toolTip : transform.Find("HUD/ToolTip").GetComponent<Text>();
    }
    private List<GameObject> Menues {
        get {
            //Returns the menues
            if (_menues.Count != 0) return _menues;

            //Finds the Menues
            for (int i = 0; i < transform.childCount; i++) _menues.Add(transform.GetChild(i).gameObject);

            //Returns the menues
            return _menues;
        }
    }
    private DialogueManager ChatManager { 
        get => _chatManager != null ? _chatManager : transform.Find("Dialogue").GetComponent<DialogueManager>();
    }

    private void Start() {
        Init();
    }

    private void Init() {
        ToolTip.text = "";              //Turns off the Text of the Tool Tip
        OpenMenu(0);                    //Closes the other menues except for the HUD
    }


    //Public Functions
    public void UpdateToolTip(string text) {
        ToolTip.text = text;
    }
    public void OpenMenu(int value) {
        for (int i = 0; i < Menues.Count; i++) Menues[i].SetActive(i == value);
    }
    public void OpenDialogue(NPC npc) {
        OpenMenu(1);
        ChatManager.OpenDialogue(npc);
    }
}
