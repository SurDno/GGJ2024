using UnityEngine;
using Mirror;
using System.Collections;

public class GravityGun : NetworkBehaviour {
    [SyncVar] public GameObject heldObject;
    [SyncVar] private Vector2 cachedMousePos;
    [SyncVar] private bool holding;
    private Vector2 direction;

    [SerializeField] private AudioClip gravityGun;
    private AudioSource gravityGunAudioSource;


    [Command]
    public void TelepathyStart(Vector2 mousePos) {
        holding = true;
        cachedMousePos = mousePos;

        LayerMask ignoredLayer = LayerMask.GetMask("Checkpoint");
        float dist = (mousePos - (Vector2)transform.position).magnitude;
        RaycastHit2D potentialTargetHit = Physics2D.Raycast((Vector2)transform.position, mousePos - (Vector2)transform.position, dist, ~ignoredLayer);

        if (potentialTargetHit.collider != null) {
            cachedMousePos = potentialTargetHit.point;
            if (heldObject == null && !IsCollidingWithObject(potentialTargetHit.collider.gameObject)) {
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
                    holding = false;
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
            
            
            Vector2 lerpNextPosition = Vector2.Lerp(heldObject.transform.position, mousePos, .05f);
            while (Vector2.Distance(lerpNextPosition, heldObject.transform.position) > 1.5f)
                lerpNextPosition = Vector2.Lerp(heldObject.transform.position, lerpNextPosition, 0.5f);

            direction = mousePos - (Vector2)heldObject.transform.position;

            float rotation = Mathf.Lerp(heldObject.transform.eulerAngles.z, 0, .1f * Time.deltaTime);
            rotation = Mathf.Floor(rotation);
            heldObject.transform.eulerAngles = new Vector3(0, 0, rotation);


            heldObject.GetComponent<Rigidbody2D>().MovePosition(lerpNextPosition);

            Debug.DrawRay(transform.position, heldObject.transform.position - transform.position, Color.red);
        }


    }

    private void Awake()
    {
        gravityGunAudioSource = gameObject.AddComponent<AudioSource>();
        gravityGunAudioSource.playOnAwake = false;
        gravityGunAudioSource.clip = gravityGun;

        gravityGunAudioSource.volume = 0.5f;
    }
    private void AdjustAudioSettings()
    {
        if (gravityGunAudioSource != null)
        {
            
            gravityGunAudioSource.volume = 0.1f; // Adjust volume here
            gravityGunAudioSource.pitch = 0.5f;  // Adjust pitch here

        }
    }

    private void PlayGravityGunSound()
    {
        if (gravityGunAudioSource == null)
        {
            gravityGunAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (gravityGunAudioSource != null)
        {
            gravityGunAudioSource.clip = gravityGun;
            gravityGunAudioSource.Play();
        }
    }


    private void DrawRay() {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = holding;

        if(heldObject != null)
            cachedMousePos = heldObject.transform.position;

        if (lineRenderer.enabled) {
            AdjustAudioSettings();
            PlayGravityGunSound();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, cachedMousePos);
        }
    }

    public void Update() {
        DrawRay();
        Debug.Log(heldObject + " " + FreezeManager.Instance.IsFrozen(heldObject));

        if (!isLocalPlayer)
            return;

        if (GetComponent<PlayerCommon>().GetRespawning() || FreezeManager.Instance.IsFrozen(this.gameObject) || 
            (heldObject != null && FreezeManager.Instance.IsFrozen(heldObject))) {
            MakeNull(false);
            return;
        }

        if (Input.GetMouseButton(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TelepathyStart(mousePos);
        } else if (Input.GetMouseButtonUp(0)) {
            MakeNull(true);
        }
    }

    [Command]
    private void MakeNull(bool saveDir) {
        holding = false;
        if (heldObject && !FreezeManager.Instance.IsFrozen(heldObject)) {
            heldObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            heldObject.GetComponent<Rigidbody2D>().velocity = saveDir ? direction : Vector2.zero;
            heldObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        }
        heldObject = null;
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
        }
    }
}
