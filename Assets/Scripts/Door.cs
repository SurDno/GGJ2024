using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour {

    [Server]
    public void Open(bool isOpening)
    {
        OpenOnClients(isOpening);
    }

    [ClientRpc] 
    public void OpenOnClients(bool isOpening) {
        GetComponentInChildren<Animator>().SetBool("isOpening", isOpening);
        GetComponentInChildren<Collider2D>().enabled = !isOpening;
    }
}
