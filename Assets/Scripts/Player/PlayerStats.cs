using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("General Stats")]
    [SerializeField] public bool CanMove = true;
    [SerializeField] private float _walkingSpeed = 1;
    [SerializeField] private float _runningSpeed = 2;
    [SerializeField] protected float currentSpeed = 1;

    [Header("Read Only Stats")]
    [SerializeField] protected LayerMask collisionMask;
    [SerializeField] private GameObject _interactor; 

    //Properites
    protected GameObject Interactor {
        get => _interactor;
        set {
            //Exits if the value is the same
            if (_interactor == value) return;

            //Sets a new value
            if (_interactor != null) _interactor.GetComponent<Interactable>().EndHover();
            if(value != null) value.GetComponent<Interactable>().Hover();

            //Sets the new Value to the Interactable 
            _interactor = value;
        }
    }

    [Header("References")]
    [SerializeField] private Animator _anim;

    //Properties
    protected Animator Anim {
        get => _anim != null ? _anim : _anim = transform.Find("Mesh").GetComponent<Animator>();
    }
    
}
