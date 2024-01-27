using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator anim;


    private void Update()
    {
      
    }
    public void Open(bool isOpening)
    {
        anim.SetBool("isOpening", isOpening);
        
       
    }

    
}
