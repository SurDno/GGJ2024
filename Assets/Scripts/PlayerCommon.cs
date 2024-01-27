using UnityEngine;
using Mirror;

public class PlayerCommon : NetworkBehaviour {
    [Header("Cached References")]
    private Rigidbody2D rb;

    [Header("Settings")]
    [Range(1f, 15f)]
    [SerializeField] private float speed;

    [Header("Current Values")]
    [SerializeField] [SyncVar] private bool movingRight;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {

        // Don't run any code for non-local player.
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.A))
            movingRight = false;
        else if (Input.GetKeyDown(KeyCode.D))
            movingRight = true;


        rb.velocity = new Vector2(movingRight ? speed : -speed, 0);
    }

}
