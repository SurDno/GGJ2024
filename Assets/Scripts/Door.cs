using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour {
    [SerializeField] private AudioClip doorOpenSound;


    [Server]
    public void Open(bool isOpening)
    {
        OpenOnClients(isOpening);
    }

    [ClientRpc] 
    public void OpenOnClients(bool isOpening) {

        GetComponentInChildren<Animator>().SetBool("isOpening", isOpening);
        GetComponentInChildren<Collider2D>().enabled = !isOpening;

        if(isOpening )
        {
            PlayDoorOpenSound();
        }
    }

    private void PlayDoorOpenSound()
    {

        AudioSource doorAudioSource = gameObject.AddComponent<AudioSource>();

        if (doorOpenSound != null)
        {
            doorAudioSource.clip = doorOpenSound;
            doorAudioSource.Play();
        }


        Destroy(doorAudioSource, doorOpenSound.length);
    }
}
