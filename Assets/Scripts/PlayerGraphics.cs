using UnityEngine;
using Mirror;

public class PlayerGraphics : NetworkBehaviour {
    [SyncVar] private bool secondPlayer;

    private void Start() {
        // Enable appropriate graphic.
        transform.Find("P1").gameObject.SetActive(secondPlayer);
        transform.Find("P2").gameObject.SetActive(!secondPlayer);

        // Enable appropriate script.
        GetComponent<FreezeRay>().enabled = secondPlayer;
        GetComponent<GravityGun>().enabled = !secondPlayer;

        gameObject.layer = secondPlayer ? LayerMask.NameToLayer("Player1") : LayerMask.NameToLayer("Player2");
    }

    [Server]
    public void SetSecondPlayer(bool value) => secondPlayer = value;
}
