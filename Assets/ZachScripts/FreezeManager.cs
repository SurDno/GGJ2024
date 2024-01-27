using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FreezeManager : NetworkBehaviour {
    public static FreezeManager Instance;

    private List<GameObject> frozenObjects = new List<GameObject>();

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

        frozenObjects.Add(collidedObject);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        Vector2 savedVelocity = rb.velocity;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(freezeTime);

        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = savedVelocity;
        frozenObjects.Remove(collidedObject);
    }

    public bool IsFrozen(GameObject obj) {
        return frozenObjects.Contains(obj);
    }

}
