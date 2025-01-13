using System;
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
    [SerializeField] private Text _money;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _fxaudioSource;
    [SerializeField] private AudioClip _typingSound, _selectSound, _switchSound, _cashSound;
    [SerializeField] private AudioClip _powerOnSound, _powerOffSound, _boxSound, _noCashSound, _appClickSound, _bombSound;

    //Properties
    private AudioSource MainSource {
        get {
            //Creates a new Audio Source if one doesn't exist already
            if (_audioSource == null) {
                var temp = gameObject.AddComponent<AudioSource>();
                _audioSource = temp;

                //Sets the Parameters
                temp.playOnAwake = false;
                temp.volume = 0.2f;
                temp.loop = true;
            }
            return _audioSource;
        }
    }
    private AudioSource FXSource {
        get {
            //Creates a new Audio Source if one doesn't exist already
            if (_fxaudioSource == null) {
                var temp = gameObject.AddComponent<AudioSource>();
                _fxaudioSource = temp;

                //Sets the Parameters
                temp.playOnAwake = false;
                temp.volume = 0.2f;
                temp.loop = false;
            }
            return _fxaudioSource;
        }
    }
    private Text ToolTip {
        get => _toolTip != null ? _toolTip : transform.Find("HUD/ToolTip").GetComponent<Text>();
    }
    private Text Money {
        get => _money != null ? _money : _money = transform.Find("HUD/Money").GetComponent<Text>();
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

    private void Awake() {
        Init();
    }

    public void Init() {
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
    public bool ToggleIpad() {
        //Closes the Menu
        if (Menues[2].activeInHierarchy) {
            PlaySound("PowerOff");
            OpenMenu(0);
            return true;
        }

        //Opens the Ipad Menu
        PlaySound("PowerOn");
        OpenMenu(2);
        return false;
        
    }
    public void OpenDialogue(ResponseOutPut npc, NPCPrompt prompt) {
        OpenMenu(1);
        ChatManager.InitializeDialogue(npc, prompt);
    }
    public void UpdateMoney(float amount) { 
        Money.text = "Money: $" + amount.ToString("0.00");
    }

    //Sound
    public void PlaySound(string type) {
        //Selects the type of sound the source will play
        switch (type) {
            case "Typing":
                MainSource.clip = _typingSound;
                MainSource.Play();
                break;
            case "Select":      FXSource.PlayOneShot(_selectSound); break;
            case "Switch":      FXSource.PlayOneShot(_switchSound); break;
            case "Cash":        FXSource.PlayOneShot(_cashSound); break;
            case "PowerOn":     FXSource.PlayOneShot(_powerOnSound); break;
            case "PowerOff":    FXSource.PlayOneShot(_powerOffSound); break;
            case "Box":         FXSource.PlayOneShot(_boxSound); break;
            case "Error":       FXSource.PlayOneShot(_noCashSound); break;
            case "App":         FXSource.PlayOneShot(_appClickSound); break;
            case "Bomb":        FXSource.PlayOneShot(_bombSound); break;

                //Nothing happens if the sound doesn't exist
            default: break;
        }
    }
    public void StopSound() {
        MainSource.Stop();
    }
}
