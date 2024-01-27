using UnityEngine;
using Mirror;

public class FreezeRay : NetworkBehaviour {
    public GameObject iceProjectilePrefab;
    public void FreezeObject() {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;
        SpawnProjectile(direction);
    }


    public void Update() {
        if (!isLocalPlayer)
            return;

        if (Input.GetMouseButtonDown(1)) {
            FreezeObject();
        }
    }

    [Command]
    void SpawnProjectile(Vector2 direction) {
        GameObject freezeProjectile = Instantiate(iceProjectilePrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(freezeProjectile);
        freezeProjectile.GetComponent<FreezeProjectile>().direction = direction;
    }
}
