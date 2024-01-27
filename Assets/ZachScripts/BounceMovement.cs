using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    // Update is called once per frame
    void Update()
    {
        rb.velocity = Vector2.up;
    }
}
