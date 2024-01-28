using UnityEngine;
using Mirror;
using System.Collections;

public class FreezeProjectile : NetworkBehaviour {
    public Vector2 direction;
    public float speedMultiplier = 5;

    private void Start() {
        if (!isServer)
            return;

        GetComponent<Rigidbody2D>().AddForce(direction * speedMultiplier, ForceMode2D.Impulse);
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime() {
        yield return new WaitForSeconds(2f);
        NetworkServer.Destroy(this.gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player1")
            return;

        if (!isServer)
            return;

        Debug.Log(collision.gameObject.name);

        NetworkServer.Destroy(gameObject);
        if (collision.gameObject.CompareTag("Pickup") || collision.gameObject.CompareTag("Player2") || collision.gameObject.CompareTag("FreezableGround")) {
            if (!FreezeManager.Instance.IsFrozen(collision.gameObject)) {
                FreezeManager.Instance.StartCoroutine(FreezeManager.Instance.FrozenObjectCountdown(collision.gameObject));
            }
        }
    }
}
