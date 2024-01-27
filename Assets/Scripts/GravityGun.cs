using UnityEngine;
using Mirror;

public class GravityGun : NetworkBehaviour {
    public GameObject heldObject;

    [Command]
    public void TelepathyStart(Vector2 mousePos) {
        RaycastHit2D potentialTargetHit = Physics2D.Raycast((Vector2)transform.position, mousePos - (Vector2)transform.position);
        Debug.DrawRay((Vector2)transform.position, mousePos - (Vector2)transform.position, Color.blue);

        if (potentialTargetHit.collider != null) {
            if (heldObject == null) {
                if (potentialTargetHit.collider.CompareTag("Pickup") && !FreezeManager.Instance.IsFrozen(potentialTargetHit.collider.gameObject)) {
                    heldObject = potentialTargetHit.collider.gameObject;
                    heldObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            } else {
                //create a layermask which holds all the layers to ignore (pickupables and the players)
                LayerMask notIgnoredLayers = LayerMask.GetMask("Environment","Player2");
                RaycastHit2D backToPlayerHit = Physics2D.Raycast(heldObject.transform.position, 
                    transform.position - heldObject.transform.position, Mathf.Infinity, notIgnoredLayers);
                Debug.DrawRay(heldObject.transform.position, transform.position - heldObject.transform.position, Color.green);

                //it's colliding with other gameObjects. shouldn't it ignore it!
                if (backToPlayerHit.collider && backToPlayerHit.collider.gameObject != this.gameObject)
                {
                    print(backToPlayerHit.collider.gameObject.name);
                    if (heldObject)
                        heldObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                    heldObject = null;
                }
            }
        }

        if (heldObject != null) {
            
            
            Vector2 lerpNextPosition = Vector2.Lerp(heldObject.transform.position, mousePos, .15f);
            Vector2 direction = lerpNextPosition - (Vector2)heldObject.transform.position;

            heldObject.GetComponent<Rigidbody2D>().MovePosition(lerpNextPosition);

            Debug.DrawRay(transform.position, heldObject.transform.position - transform.position, Color.red);
        }


    }
    public void Update() {
        if (!isLocalPlayer)
            return;
        if (Input.GetMouseButton(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TelepathyStart(mousePos);
        } else if (Input.GetMouseButtonUp(0)) {
            MakeNull();
        }
    }

    [Command]
    private void MakeNull() {
        if(heldObject)
           heldObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        heldObject = null;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == heldObject)
        {
            //Still need to stop being hit by it
            MakeNull();
        }
    }
}
