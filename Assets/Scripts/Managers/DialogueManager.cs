using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Text _name;
    [SerializeField] private Text _mainText;

    //Properties
    private Text Name {
        get => _name != null ? _name : _name = transform.Find("Name/Text").GetComponent<Text>();
    }
    private Text MainText { 
        get => _mainText != null ? _mainText : _mainText = transform.Find("MainText").GetComponent<Text>(); 
    }

    [Header("Read-Only Stats")]
    [SerializeField] private NPC _currentNPC;
    [SerializeField] private int _state = 0;
    [SerializeField] private bool _finishedTyping = false;
    [SerializeField] Coroutine _typingCoroutine; 

    //Starts the Dialogue tree
    public void OpenDialogue(NPC npc) {
        //Exits becuase nothing was sent over
        if (npc.DialogueTree.Count == 0) {
            StopDialogue();
            return;
        }

        //Initializes the vlaues
        _state = 0;
        _currentNPC = npc;

        //Runs the Dialogue
        RunDialogue();
    }
    private void RunDialogue() {
        //Checks to make sure it is not the end
        if (_state == -1 || _state + 1 > _currentNPC.DialogueTree.Count) {
            StopDialogue();
            return;
        }
        
        //Starts the typing
        Name.text = _currentNPC.DialogueTree[_state].Name;
        _typingCoroutine = StartCoroutine(TypeText(_currentNPC.DialogueTree[_state].Text));

    }
    private void Update() {
        //Continues if the key is pressed
        if (Input.GetKeyDown(KeyCode.E)) {
            if (_finishedTyping) {
                _state++;
                RunDialogue();
            } else {
                //Stops the typing co-routine and sets the text to the full text
                StopCoroutine(_typingCoroutine);
                MainText.text = _currentNPC.DialogueTree[_state].Text;
                _finishedTyping = true;
            }
        }
    }

    IEnumerator TypeText(string text) {
        //Initializes Variables
        var newString = "";
        _finishedTyping = false;

        //Types in the text
        for (int i = 0; i < text.Length; i++) {
            newString += text[i];
            MainText.text = newString;
            yield return new WaitForSeconds(0.1f);
        }

        _finishedTyping = true;
    }

    //Stops the Dialogue
    public void StopDialogue() {
        GameManager.Manager.StopDialogue();
    }
}
