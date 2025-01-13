using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffFire : MonoBehaviour {
    [Header("References")]
    [SerializeField] private NPC _npc;
    [SerializeField] private AudioSource _audioSource;

    private void FixedUpdate() {
        //Turns itself off if the player passed the quest
        if (!_npc._passedQuest) return;

        //Edits the Tweaker
        _npc.Anim.SetBool("Talking", true);
        _npc._animate = false;

        //Turns off the Fire and Sets the Music
        _audioSource.clip = Resources.Load<AudioClip>("Music/Tweaker_Good");
        _audioSource.Play();
        gameObject.SetActive(false);
    }
}
