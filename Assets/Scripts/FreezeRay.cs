using UnityEngine;
using Mirror;

public class FreezeRay : NetworkBehaviour {
    public GameObject iceProjectilePrefab;

    [SerializeField] private AudioClip freezeRay;
    private AudioSource freezeRayAudioSource;

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

    private void Awake()
    {
        freezeRayAudioSource = gameObject.AddComponent<AudioSource>();
        freezeRayAudioSource.playOnAwake = false;
        freezeRayAudioSource.clip = freezeRay;

        freezeRayAudioSource.volume = 0.7f;  
    }

    private void PlayFreezeRaySound()
    {
        if (freezeRayAudioSource != null)
        {
            freezeRayAudioSource.Play();
        }
    }

    [Command]
    void SpawnProjectile(Vector2 direction) {
        PlayFreezeRaySound();
        GameObject freezeProjectile = Instantiate(iceProjectilePrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(freezeProjectile);
        freezeProjectile.GetComponent<FreezeProjectile>().direction = direction;
    }
}
