using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("General Camera Stats")]
    [SerializeField] private GameObject _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector2 _bounds;

    private void LateUpdate() {
        //Follows the target
        FollowTarget();
    }

    //Follows the target
    private void FollowTarget() {
        //Follows the Position
        var newPos = _target.transform.position + _offset;

        //Exits if it is out of bounds
        if (newPos.x < _bounds.x) newPos.x = _bounds.x;
        else if (newPos.x > _bounds.y) newPos.x = _bounds.y;

        //Moves the Camera Towards the Position
        transform.position = Vector3.MoveTowards(transform.position, newPos, 1);
    }
}
