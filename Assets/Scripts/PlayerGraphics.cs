using UnityEngine;
using Mirror;
using static UnityEngine.GraphicsBuffer;

public class PlayerGraphics : NetworkBehaviour {
    [SyncVar] private bool secondPlayer;
    private SpriteRenderer target;
    private void Start() {
        // Enable appropriate graphic.
        if (secondPlayer) {
            Destroy(transform.Find("P2").gameObject);
            target = transform.Find("P1").Find("GravityGun").GetComponent<SpriteRenderer>();
        } else {
            Destroy(transform.Find("P1").gameObject);
            target = transform.Find("P2").Find("FreezeRay").GetComponent<SpriteRenderer>();
        }

        // Enable appropriate script.
        GetComponent<FreezeRay>().enabled = !secondPlayer;
        GetComponent<GravityGun>().enabled = secondPlayer;

        gameObject.layer = !secondPlayer ? LayerMask.NameToLayer("Player1") : LayerMask.NameToLayer("Player2");
    }
    private void Update() {
        float angleDefault = Quaternion.Angle(target.transform.rotation, Quaternion.Euler(0f, 0, 0f));
        float angleFlipped = Quaternion.Angle(target.transform.rotation, Quaternion.Euler(0f, 0, 180f));

        target.flipY = (Mathf.Abs(angleDefault) > Mathf.Abs(angleFlipped));

        Vector2 potentialDestination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = (potentialDestination - (Vector2)transform.position).normalized;
        target.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg);
    }

    [Server]
    public void SetSecondPlayer(bool value) => secondPlayer = value;
}
