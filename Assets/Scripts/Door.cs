using UnityEngine;
using Mirror;
using System.Collections;

public class Door : NetworkBehaviour {
    [SerializeField] private AudioClip doorOpenSound;
    [SyncVar] private bool doorFrozen;

    [Server]
    public void Open(bool isOpening)
    {
        OpenOnClients(isOpening);
    }

    private void Update() {
        Animator anim = GetComponentInChildren<Animator>();

        if (isServer)
            doorFrozen = FreezeManager.Instance.IsFrozen(anim.gameObject);
    }

    [ClientRpc] 
    public void OpenOnClients(bool isOpening) {
        Animator anim = GetComponentInChildren<Animator>();

        if (doorFrozen) {
            StartCoroutine(WaitForUnfreeze(isOpening));
            return;
        }
        anim.SetBool("isOpening", isOpening);
        //GetComponentInChildren<Collider2D>().enabled = !isOpening;

        if (isOpening) {
            PlayDoorOpenSound();
        }
    }


    private void PlayDoorOpenSound() {

        AudioSource doorAudioSource = gameObject.AddComponent<AudioSource>();

        if (doorOpenSound != null) {
            doorAudioSource.clip = doorOpenSound;
            doorAudioSource.Play();
        }


        Destroy(doorAudioSource, doorOpenSound.length);
    }

    [Client]
    IEnumerator WaitForUnfreeze(bool isOpening) {
        Animator anim = GetComponentInChildren<Animator>();

        while (doorFrozen)
            yield return null;

      anim.SetBool("isOpening", isOpening);
        //GetComponentInChildren<Collider2D>().enabled = !isOpening;

        if (isOpening) {
            PlayDoorOpenSound();
        }
    }
}
