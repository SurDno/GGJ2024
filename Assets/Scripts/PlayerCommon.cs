using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerCommon : NetworkBehaviour {
    [Header("Cached References")]
    private Rigidbody2D rb;
    private BoxCollider2D bCollider;

    [Header("Settings")]
    [Range(1f, 15f)]
    [SerializeField] private float speed = 6f;
    [Range(0f, 1f)]
    [SerializeField] private float acceleration = 0.008f;
    [Range(5f, 15f)]
    [SerializeField] private float maxFallSpeed = 9.81f;
    [Range(0f, 3f)]
    [SerializeField] private float timeToFullFallSpeed = 0.7f;
    [Range(5f, 20f)]
    [SerializeField] private float maxJumpSpeed = 6f;
    [Range(0f, 2f)]
    [SerializeField] private float timeToFullJump = 1f;

    [Header("Current Values")]
    [SerializeField][SyncVar] private bool movingRight, moving, preparingJump, afterJump, isRespawning;
    [SerializeField] private float fallSpeed, timer;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        bCollider = GetComponent<BoxCollider2D>(); 
    }

    void Update() {
        // Don't run any code for non-local player.
        if (!isLocalPlayer)
            return;

        if (isRespawning)
            return;

        if (FreezeManager.Instance.IsFrozen(this.gameObject))
            return;

        if (Input.GetKey(KeyCode.H)) { StartCoroutine(Respawn()); }
        if (Input.GetKey(KeyCode.Space) && !preparingJump && IsGrounded())
            StartCoroutine(PrepareJump());

        moving = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !preparingJump;
        movingRight = Input.GetKey(KeyCode.D);

        fallSpeed = IsGrounded() ? 0 : fallSpeed + maxFallSpeed / timeToFullFallSpeed * Time.deltaTime;
        fallSpeed = Mathf.Clamp(fallSpeed, -maxJumpSpeed, maxFallSpeed);

        float horizontalVelocity = Mathf.Lerp(rb.velocity.x, moving ? (movingRight ? speed : -speed) : 0, acceleration);
        float verticalVelocity = -fallSpeed;

        rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

    IEnumerator PrepareJump() {
        preparingJump = true; 

        while(Input.GetKey(KeyCode.Space)) {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, timeToFullJump);
            yield return null;
        }

        preparingJump = false;

        fallSpeed = -Mathf.Lerp(0, maxJumpSpeed, Mathf.InverseLerp(0, timeToFullJump, timer));
        timer = 0;

        // Dity hack so that we're not grounded immediately after jumping.
        afterJump = true;
        yield return new WaitForSeconds(0.1f);
        afterJump = false;
    }

    bool IsGrounded() {
        if (afterJump)
            return false;

        ContactPoint2D[] currentContacts = new ContactPoint2D[256];
        bCollider.GetContacts(currentContacts);

        if (currentContacts.Length == 0)
            return false;

        for (int i = 0; i < currentContacts.Length; i++)
            if (currentContacts[i].collider && (currentContacts[i].collider.CompareTag("Ground") || currentContacts[i].collider.CompareTag("Pickup")))
                if (Mathf.Abs(Vector2.Angle(currentContacts[i].normal, Vector2.up)) <= 45)
                    return true;

        return false;
    }

    [Command]
    public void StartRespawn() => RespawnOnClients();

    [ClientRpc]
    private void RespawnOnClients() => StartCoroutine(Respawn());

    public IEnumerator Respawn() {
        SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        rb.velocity = Vector2.zero;

        isRespawning = true;
        for (int i = 0; i < 7; i++)
        {
            if(i == 2)
                transform.position = GameObject.Find("RespawnPoint").transform.position;

            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
        isRespawning = false;
    }


    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log(collision.contacts[0].normal.y);
        if (collision.gameObject.CompareTag("Pickup") && collision.contacts[0].normal.y < -0.5f) {
            RespawnOnClients();
        }
    }

    public bool GetRespawning() => isRespawning;
}
