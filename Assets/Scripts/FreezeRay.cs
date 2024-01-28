using UnityEngine;
using Mirror;

public class FreezeRay : NetworkBehaviour {
    public GameObject iceProjectilePrefab;
    public void FreezeObject() {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        SpawnProjectile(direction * 3);
    }


    public void Update() {
        if (GetComponent<PlayerCommon>().GetRespawning())
            return;

        if (!isLocalPlayer)
            return;

        if (Input.GetMouseButtonDown(0)) {
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
