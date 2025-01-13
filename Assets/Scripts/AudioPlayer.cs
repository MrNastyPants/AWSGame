using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public void PlayAudio() {
        //Exits if there is no audio source
        if (_audioSource == null) return;

        //Plays the Audio
        _audioSource.Play();
    }
}
