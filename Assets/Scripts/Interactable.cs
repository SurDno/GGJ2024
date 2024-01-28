using System.Collections.Generic;
using UnityEngine;
using Mirror;

public  class Interactable : NetworkBehaviour {
    public bool isInteractable = true;
    public List<GameObject> interactingGO = new List<GameObject>();
   
    [Header("What it does")]
    public Door[] affectedDoors;
    protected void InteractWithObject(GameObject gO) {
        foreach (Door door in affectedDoors)
            door.Open(true);

        isInteractable = false;
    }

    protected void StopInteractingWithObject()  {
        foreach(Door door in affectedDoors)
            door.Open(false);
        
        isInteractable = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!isServer)
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1") || 
            collision.gameObject.layer == LayerMask.NameToLayer("Player2") ||
            collision.gameObject.layer == LayerMask.NameToLayer("PickupObject")) {
            interactingGO.Add(collision.gameObject);

            if (isInteractable)
                InteractWithObject(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (!isServer)
            return;

        if (interactingGO.Contains(collision.gameObject))
            interactingGO.Remove(collision.gameObject);

        if (interactingGO.Count == 0)
            StopInteractingWithObject();
    }
}
