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
                }
            } else {

                RaycastHit2D backToPlayerHit = Physics2D.Raycast(heldObject.transform.position, transform.position - heldObject.transform.position);
                Debug.DrawRay(heldObject.transform.position, transform.position - heldObject.transform.position, Color.green);

                if (backToPlayerHit.collider.gameObject != this.gameObject && !backToPlayerHit.collider.CompareTag("Pickup")) {
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
        if (Input.GetMouseButton(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TelepathyStart(mousePos);
        } else if (Input.GetMouseButtonUp(0)) {
            MakeNull();
        }
    }

    [Command]
    private void MakeNull() {
        heldObject = null;
    }
}
