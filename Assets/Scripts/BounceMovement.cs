using Mirror;
using UnityEngine;

public class BounceMovement : NetworkBehaviour {
    private Vector2 p1, p2;
    public Transform otherPos;
    public float moveDelay;
    public float speedMultiplier;

    private void Start() {
        p1 = transform.position;
        p2 = otherPos.transform.position;
    }
    void Update()  {
        if (!isServer)
            return;

        if (FreezeManager.Instance.IsFrozen(this.gameObject))
             return;

        float t = Mathf.PingPong(Time.time * speedMultiplier, 1f + moveDelay * 2) - moveDelay;
        
        if(t >= 0 && t <= 1)
            transform.position = Vector2.Lerp(p1, p2, t);

    }
}
