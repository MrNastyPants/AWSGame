using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("General Camera Stats")]
    [SerializeField] private GameObject _target;

    private void LateUpdate() {
        //Follows the target
        FollowTarget();
    }

    //Follows the target
    private void FollowTarget() {
        var newPos = new Vector3(_target.transform.position.x, _target.transform.position.y, -10);
        float speed = 0.25f + Vector2.Distance(newPos, transform.position);
        transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
    }
}
