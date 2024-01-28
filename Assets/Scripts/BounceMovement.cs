using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BounceMovement : MonoBehaviour{

    public Vector2 p1, p2;
    public float moveDelay;
    public float speedMultiplier;

    /* private void Update()
     {
         if (Input.GetMouseButtonDown(0))
         {
             StartCoroutine(MoveToNextPoint());
         }
     }
     public IEnumerator MoveToNextPoint()
     {
         float t = Mathf.PingPong(Time.time * speedMultiplier, 1f);
         transform.position = Vector2.Lerp(transform.position, movementPoints[movePointIndex].position, t);
         Debug.Log((movementPoints[movePointIndex].position - transform.position).magnitude);
         if (t >= 1)
         {
             movePointIndex++;
             if(movePointIndex >= movementPoints.Length)
             {
                 movePointIndex = 0;
             }
         }
         StartCoroutine(MoveToNextPoint());
         yield return null;

     }*/

    

    void Update()
    {
        // if (FreezeManager.Instance.IsFrozen(this.gameObject))
        //     return;

        //PingPong between 0 and 1
        float t = Mathf.PingPong(Time.time * speedMultiplier, 1f+moveDelay);
        
        //implement delay (for platforms that jut out)
        if(t > 1)
        {

        }
        else
        {
            transform.position = Vector2.Lerp(p1, p2, t);
        }
       
       


    }
}
