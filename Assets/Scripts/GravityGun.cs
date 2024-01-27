using UnityEngine;
using Mirror;
using System.Collections;

public class GravityGun : NetworkBehaviour {
    [SyncVar] public GameObject heldObject;
    [SyncVar] private Vector2 cachedMousePos;
    private Vector2 direction;
    private bool coolingDown;

    [Command]
    public void TelepathyStart(Vector2 mousePos) {
        cachedMousePos = mousePos;

        float dist = (mousePos - (Vector2)transform.position).magnitude;
        RaycastHit2D potentialTargetHit = Physics2D.Raycast((Vector2)transform.position, mousePos - (Vector2)transform.position, dist);

        if (potentialTargetHit.collider != null) {
            if (heldObject == null && !coolingDown && !IsCollidingWithObject(potentialTargetHit.collider.gameObject)) {
                if (potentialTargetHit.collider.CompareTag("Pickup") && !FreezeManager.Instance.IsFrozen(potentialTargetHit.collider.gameObject)) {
                    heldObject = potentialTargetHit.collider.gameObject;
                    heldObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    heldObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    heldObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                }
            } else if (heldObject) {
                //create a layermask which holds all the layers to ignore (pickupables and the players)
                LayerMask notIgnoredLayers = LayerMask.GetMask("Environment","Player2");
                RaycastHit2D backToPlayerHit = Physics2D.Raycast(heldObject.transform.position, 
                    transform.position - heldObject.transform.position, Mathf.Infinity, notIgnoredLayers);
                Debug.DrawRay(heldObject.transform.position, transform.position - heldObject.transform.position, Color.green);

                //it's colliding with other gameObjects. shouldn't it ignore it!
                if (backToPlayerHit.collider && backToPlayerHit.collider.gameObject != this.gameObject)
                {
                    if (heldObject) {
                        heldObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                        heldObject.GetComponent<Rigidbody2D>().velocity = direction;
                        heldObject.GetComponent<Rigidbody2D>().gravityScale = 1;
                    }
                    heldObject = null;
                }
            }
        }

        if (heldObject != null) {
            
            
            Vector2 lerpNextPosition = Vector2.Lerp(heldObject.transform.position, mousePos, .15f);
            direction = mousePos - (Vector2)heldObject.transform.position;

            float rotation = Mathf.Lerp(heldObject.transform.eulerAngles.z, 0, .1f * Time.deltaTime);
            rotation = Mathf.Floor(rotation);
            heldObject.transform.eulerAngles = new Vector3(0, 0, rotation);

            heldObject.GetComponent<Rigidbody2D>().MovePosition(lerpNextPosition);

            Debug.DrawRay(transform.position, heldObject.transform.position - transform.position, Color.red);
        }


    }

    private void DrawRay() {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = Input.GetMouseButton(0);

        if(heldObject != null)
            cachedMousePos = heldObject.transform.position;

        if (lineRenderer.enabled) {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, cachedMousePos);
        }
    }

    public void Update() {
        DrawRay();

        if (!isLocalPlayer)
            return;
        if (Input.GetMouseButton(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TelepathyStart(mousePos);
        } else if (Input.GetMouseButtonUp(0)) {
            MakeNull(true);
        }
    }

    [Command]
    private void MakeNull(bool saveDir) {
        if (heldObject) {
            heldObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            heldObject.GetComponent<Rigidbody2D>().velocity = saveDir ? direction : Vector2.zero;
            heldObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        }
        heldObject = null;
    }

    IEnumerator Cooldown() {
        coolingDown = true;
        yield return new WaitForSeconds(0.5f);
        coolingDown = false;
    }

    private bool IsCollidingWithObject(GameObject obj) {
        ContactPoint2D[] currentContacts = new ContactPoint2D[256];
        GetComponent<Collider2D>().GetContacts(currentContacts);

        if (currentContacts.Length == 0)
            return false;

        for (int i = 0; i < currentContacts.Length; i++)
            if (currentContacts[i].collider && currentContacts[i].collider.gameObject == obj && currentContacts[i].normal.y > 0.5f)
                return true;

        return false;
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == heldObject)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            if (heldObject) {
                heldObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                heldObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                heldObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            }
            heldObject = null;
            //StartCoroutine(Cooldown());
        }
    }
}
