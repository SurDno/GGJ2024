using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public  class Interactable : MonoBehaviour
{
    public bool isInteractable = true;
    public List<GameObject> interactingGO = new List<GameObject>();
   
    [Header("What it does")]
    public GameObject[] affectedDoors;
    public List<Door> allDoors = new List<Door>();

    private void Start()
    {
        foreach(GameObject door in affectedDoors)
        {
            allDoors.Add(door.transform.GetChild(0).GetComponent<Door>());
        }
    }
    protected void InteractWithObject(GameObject gO)
    {
        foreach (Door door in allDoors)
        {
            door.Open(true);
        }
        isInteractable = false;
        
        //do Something
    }

    protected void StopInteractingWithObject()
    {
        foreach(Door door in allDoors)
        {
            door.Open(false);
        }
        
        isInteractable = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1") || 
            collision.gameObject.layer == LayerMask.NameToLayer("Player2") ||
            collision.gameObject.layer == LayerMask.NameToLayer("PickupObject"))
           {
            interactingGO.Add(collision.gameObject);
            if (isInteractable)
            {
                InteractWithObject(collision.gameObject);
            }
        }
       
            
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (interactingGO.Contains(collision.gameObject))
        {
            interactingGO.Remove(collision.gameObject);
        }
        //if the player
        if (interactingGO.Count == 0)
        {
            StopInteractingWithObject();

        }
    }
}
