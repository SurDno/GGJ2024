using UnityEngine;
using Mirror;
using System.Collections;

public class Door : NetworkBehaviour {
    [SerializeField] private AudioClip doorOpenSound;


    [Server]
    public void Open(bool isOpening)
    {
        OpenOnClients(isOpening);
    }

    private void Update() {
        Animator anim = GetComponentInChildren<Animator>();
        Debug.Log(FreezeManager.Instance.IsFrozen(anim.gameObject) + " " + FreezeManager.Instance.IsFrozen(this.gameObject));

    }

    [ClientRpc] 
    public void OpenOnClients(bool isOpening) {
        Animator anim = GetComponentInChildren<Animator>();

        if (FreezeManager.Instance.IsFrozen(anim.gameObject)) {
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

        while (FreezeManager.Instance.IsFrozen(anim.gameObject))
            yield return null;

      anim.SetBool("isOpening", isOpening);
        //GetComponentInChildren<Collider2D>().enabled = !isOpening;

        if (isOpening) {
            PlayDoorOpenSound();
        }
    }
}
