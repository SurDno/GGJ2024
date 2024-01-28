using System.Collections.Generic;
using UnityEngine;
using Mirror;

public  class Interactable : NetworkBehaviour
{
    public bool isInteractable = true;
    public List<GameObject> interactingGO = new List<GameObject>();

    [Header("What it does")]
    public Door[] affectedDoors;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip buttonSound;
    private AudioSource buttonAudioSource;

    private void Start()
    {
        buttonAudioSource = gameObject.AddComponent<AudioSource>();
        buttonAudioSource.playOnAwake = false;
        buttonAudioSource.clip = buttonSound;

        // Initialize other components or settings as needed
    }

    protected void InteractWithObject(GameObject gO)
    {
        foreach (Door door in affectedDoors)
        {
            door.Open(true);
        }

        isInteractable = false;

        // Play sound when interacting
        PlayCollisionSound();
    }

    protected void StopInteractingWithObject()
    {
        foreach (Door door in affectedDoors)
        {
            door.Open(false);
        }

        isInteractable = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer)
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player1") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Player2") ||
            collision.gameObject.layer == LayerMask.NameToLayer("PickupObject"))
        {
            interactingGO.Add(collision.gameObject);

            if (isInteractable)
                InteractWithObject(collision.gameObject);

            // Play sound when collision occurs
            PlayCollisionSound();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isServer)
            return;

        if (interactingGO.Contains(collision.gameObject))
            interactingGO.Remove(collision.gameObject);

        if (interactingGO.Count == 0)
            StopInteractingWithObject();
    }

    private void PlayCollisionSound()
    {
        if (buttonAudioSource != null)
        {
            buttonAudioSource.Play();
        }
    }
}