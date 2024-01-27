using Mirror;
using UnityEngine;

public class BounceMovement : NetworkBehaviour{
    public Rigidbody2D rb;

    [Client]
    void Start() {
        rb.gravityScale = 0;
    }
}
