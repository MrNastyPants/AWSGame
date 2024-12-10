using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("General Camera Stats")]
    [SerializeField] private GameObject _target;
    [SerializeField] private Vector3 _offset; 

    private void LateUpdate() {
        //Follows the target
        FollowTarget();
    }

    //Follows the target
    private void FollowTarget() {
        //Follows the Position
        var newPos = _target.transform.position + _offset;
        transform.position = Vector3.MoveTowards(transform.position, newPos, 1);
    }
}
