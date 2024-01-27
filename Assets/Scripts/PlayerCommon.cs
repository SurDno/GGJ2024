using UnityEngine;
using Mirror;

public class PlayerCommon : NetworkBehaviour {
    [Header("Cached References")]
    private Rigidbody2D rb;
    private BoxCollider2D bCollider;

    [Header("Settings")]
    [Range(1f, 15f)]
    [SerializeField] private float speed;
    [Range(5f, 15f)]
    [SerializeField] private float maxFallSpeed = 9.81f;
    [Range(0f, 3f)]
    [SerializeField] private float timeToFullFallSpeed;

    [Header("Current Values")]
    [SerializeField] [SyncVar] private bool movingRight;
    [SerializeField] private float fallSpeed = 0f;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        bCollider = GetComponent<BoxCollider2D>(); 
    }

    void Update() {

        // Don't run any code for non-local player.
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.A))
            movingRight = false;
        else if (Input.GetKeyDown(KeyCode.D))
            movingRight = true;


        if (IsGrounded())
            fallSpeed = 0;
        else
            fallSpeed = Mathf.Min(maxFallSpeed, fallSpeed + maxFallSpeed / timeToFullFallSpeed * Time.deltaTime);

        float horizontalVelocity = movingRight ? speed : -speed;
        float verticalVelocity = -fallSpeed;

        rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

    bool IsGrounded() {
        ContactPoint2D[] currentContacts = new ContactPoint2D[256];
        bCollider.GetContacts(currentContacts);

        if (currentContacts.Length == 0)
            return false;

        for (int i = 0; i < currentContacts.Length; i++)
            if (currentContacts[i].collider && currentContacts[i].collider.CompareTag("Ground"))
                if (Mathf.Abs(Vector2.Angle(currentContacts[i].normal, Vector2.up)) <= 45)
                    return true;

        return false;
    }
}
