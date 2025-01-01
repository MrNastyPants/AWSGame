using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _workSpace;
    [SerializeField] public ResponseOutPut _file;

    //Properties
    protected Transform WorkSpace {
        get => _workSpace != null ? _workSpace : _workSpace = transform.Find("WorkSpace");
    }


    [Header("Output")]
    [SerializeField] protected Dialogue[] OutPutResponse;
    [SerializeField] protected GameObject[] ResponseUnits;
}
