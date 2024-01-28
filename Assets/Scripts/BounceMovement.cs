using Mirror;
using UnityEngine;

public class BounceMovement : NetworkBehaviour {
    public Vector2 p1, p2;
    public float moveDelay;
    public float speedMultiplier;

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
