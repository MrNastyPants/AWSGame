using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _workSpace;
    [SerializeField] private GameObject _startResponse;

    //Properties
    protected Transform WorkSpace {
        get => _workSpace != null ? _workSpace : _workSpace = transform.Find("WorkSpace");
    }
    protected GameObject StartResponse {
        get => _startResponse != null ? _startResponse : _startResponse = transform.Find("WorkSpace/Start").gameObject;
    }


    [Header("Output")]
    [SerializeField] protected List<Dialogue> OutPutResponse = new List<Dialogue>();
    [SerializeField] protected GameObject[] ResponseUnits;
}
