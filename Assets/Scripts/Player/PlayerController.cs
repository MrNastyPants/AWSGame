using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerStats
{
    //Readonly Variables
    private bool moving = false;
    
    public void Update() {
        //Exits if the player cannot move
        if (!CanMove) return;

        //Moves the Player
        Movement();

        //Interacts with the door.
        if (Input.GetKeyDown(KeyCode.E) && Interactor != null) {
            Interactor.GetComponent<Interactable>().Interact();
        }
    }

    //Moves the player
    private void Movement() {
        //Initialize values
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        //Sets the Animation
        Anim.SetBool("Walking", moving);

        //Returns if the values are zero
        if (moving || (horizontal == 0 && vertical == 0)) return;

        //Creates the new direction
        Vector3Int newDir = vertical == 0 ? new Vector3Int(horizontal > 0f ? 1 : -1, 0, 0) : new Vector3Int(0, vertical > 0f ? 1 : -1, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(newDir.x, newDir.y), 1, collisionMask);

        //Interactable Item Detected
        if (hit.collider && hit.collider.GetComponent<Interactable>() != null) Interactor = hit.collider.gameObject;
        else Interactor = null;

        //Moves towards the new direction
        if (hit.collider == null) {
            //Starts the Movement
            StartCoroutine(Move(newDir));

            //Sets the Animation
            Anim.SetFloat("SpeedX", newDir.x);
            Anim.SetFloat("SpeedY", newDir.y);
        } 
    }

    public IEnumerator Move(Vector3Int direction) {
        //Starts the movement
        moving = true;

        //Initialize the values
        var target = transform.position + direction;

        do {
            //Moves a little bit towards the target
            transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * 0.075f);
            yield return new WaitForSeconds(0.01f);

        } while (Vector3.Distance(transform.position, target) > 0.01f);

        //Sets the position to the target
        transform.position = target;

        //Updates the Order in layer if the Y direction moved
        if(direction.y != 0)
            transform.Find("Mesh").GetComponent<SpriteRenderer>().sortingOrder = 0 - Mathf.RoundToInt(transform.position.y);

        //Turns off the walking animation
        moving = false;
    }
}
