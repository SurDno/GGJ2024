using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FreezeManager : NetworkBehaviour {
    public static FreezeManager Instance;

    private List<GameObject> frozenObjects = new List<GameObject>();
    [SerializeField] private GameObject freezeImage;

    [Header("Settings")]
    [SerializeField] private float freezeTime;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    [Server]
    public IEnumerator FrozenObjectCountdown(GameObject collidedObject) {

        Rigidbody2D rb = collidedObject.GetComponent<Rigidbody2D>();

        GameObject freezeImageInst = Instantiate(freezeImage, collidedObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(freezeImageInst);

        StoreFrozenObject(collidedObject);
        Vector2 savedVelocity = Vector2.zero;
        if (!collidedObject.GetComponent<BounceMovement>()) {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            savedVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }

        yield return new WaitForSeconds(freezeTime);

        if (collidedObject.GetComponent<BounceMovement>()) {
            //...
        } else if (collidedObject.GetComponent<PlayerCommon>()) {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.velocity = savedVelocity;
        } else {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 1;
        }

        Destroy(freezeImageInst);

        UnstoreFrozenObject(collidedObject);
    }

    [ClientRpc]
    public void StoreFrozenObject(GameObject obj) => frozenObjects.Add(obj);

    [ClientRpc]
    public void UnstoreFrozenObject(GameObject obj) => frozenObjects.Remove(obj);



    public bool IsFrozen(GameObject obj) {
        return frozenObjects.Contains(obj);
    }

}
