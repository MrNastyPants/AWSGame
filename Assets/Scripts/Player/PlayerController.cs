using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : PlayerStats
{
    //Readonly Variables
    private bool moving = false;
    
    public void Update() {
        //Opens the Ipad
        if (Input.GetKeyDown(KeyCode.Tab) && !GameManager.Manager.IsTalking()) {
            //Toggles the Can Move
            CanMove = GameManager.Manager.HUD.ToggleIpad();
        }

        //Exits if the player cannot move
        if (!CanMove) return;

        //Moves the Player
        Movement();

        //Interacts with the interactable.
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

        //Creates the new direction and Rotates the player
        Vector3Int newDir = vertical == 0 ? new Vector3Int(horizontal > 0f ? 1 : -1, 0, 0) : new Vector3Int(0, 0, vertical > 0f ? 1 : -1);
        Mesh.transform.rotation = Quaternion.LookRotation(newDir);

        //Block Item Detected
        if (Physics.Raycast(transform.position + (Vector3.up * 0.25f), newDir, out var hit, 1, collisionMask)) {
            //Interactable Item detected
            if(hit.collider.GetComponent<Interactable>() != null) Interactor = hit.collider.gameObject;
            return;
        }

        //Sets the Interactor to Null
        Interactor = null;

        //Starts the Movement
        StartCoroutine(Move(newDir));
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

        //Finished up Moving
        DoneMoving();
    }
    private void DoneMoving() {
        //Sets Values
        moving = false;

        //Checks the Camera to make sure nothing is blocking it
        var direction = (_camera.position - transform.position).normalized;
        var allBlocks = Physics.BoxCastAll(transform.position + (Vector3.up * 0.5f) + direction, new Vector3(1f, 1.5f, 1), direction, 
            Quaternion.identity, Vector3.Distance(transform.position, _camera.position), _wallMask);

        //Removes the old ones
        foreach (RaycastHit obj in _blockingObjects) {
            //Continues if it was hit again
            if (allBlocks.Contains(obj)) continue;

            //Respawns the object
            obj.transform.GetComponent<WallHider>().HideWalls(false);
        }

        foreach (RaycastHit obj in allBlocks) {
            //Checks to see if it has the component
            if (!obj.transform.GetComponent<WallHider>()) continue;

            //Hides the Walls and adds it to the object
            obj.transform.GetComponent<WallHider>().HideWalls(true);
            _blockingObjects.Add(obj);
        }
    }

    //Public Functions
    public void ChangeCarry(bool carrying) {
        //Sets the Animation to change
        Anim.SetBool("Holding_Item", carrying);

        //Enables the Box that the player will carry
        Package.SetActive(carrying);
    }
}
