using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeProjectile : MonoBehaviour
{
    public Vector2 direction;
    public float speedMultiplier = 5;
    private void Start()
    {
        //transform.GetComponent<Rigidbody>().MovePosition(direction);
        Debug.Log(direction);
        GetComponent<Rigidbody2D>().AddForce(direction * speedMultiplier, ForceMode2D.Impulse);
        Destroy(gameObject, 2f);
        //  //maybe instead a rb.move on update?
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        //Only colliding with player (as instantiates there, nothing else)
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Pickup"))
        {
            if (!FreezeManager.instance.frozenObjects.Contains(collision.gameObject))
            {
                FreezeManager.instance.StartFrozenObjectCountdown(collision.gameObject);
            }
            FreezeManager.instance.StartFrozenObjectCountdown(collision.gameObject);
            Destroy(gameObject);//destroy if touches
        }
        
    }
    

}
