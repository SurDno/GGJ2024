using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{//NEEDS TO INHERIT FROM PlayerCommon
    [Header("Common vars")]
    public Rigidbody2D rb;

    [Header("For Freeze ability")]
    public GameObject iceProjectilePrefab;
    public bool isFrozen;
    public float freezeGunCooldown;

    [Header("For Gravity common ability")]
    public int gravityMultiplier = 1; //if it's -1 then gravity affects rigidbody other way


    [Header("For Telepathy ability")]
    public GameObject heldObject;
    //Specific player controls:


    [Header("For Interact ability")]
    public bool isInteracting;

    //Common Ability 1: Changing gravity
    public void switchGravity()
    {
        gravityMultiplier *= -1; //just switches the gravity multiplier (or we *= -1 the rigidbody gravity)
    }

    //Ability 2: Freezing objects
    public void FreezeObject()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject freezeProjectile = Instantiate(iceProjectilePrefab, transform.position,Quaternion.identity,transform);
        Vector2 direction = mousePos - transform.position;
        freezeProjectile.transform.parent = null;
       freezeProjectile.GetComponent<FreezeProjectile>().direction = direction;

    }

    
    //Ability 3: Telepathy
    public void TelepathyStart()
    {
        //do a raycast from transform to mousePosition
        var camera = Camera.main;

        //get mousePosition in world
        var mousePos = (Vector2)camera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, mousePos); //Currently hitting the wall first not the gameobject in front?
        Debug.DrawRay((Vector2)transform.position, mousePos - (Vector2)transform.position, Color.blue);
        //Do 2 different types of raycasts
        //Find your heldObject -> Case: Don't have a held object yet
        if (heldObject == null)
        {
            //gets the held object
            if (hit.collider != null)
            {
                
                if (hit.collider.CompareTag("Pickup"))
                {
                    heldObject = hit.collider.gameObject;
                }
               
            }

        }
        //2nd raycast is from player to heldObject -> Case: Object is behind wall so drop it
        else
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.collider);
                //if it collides with something that ISNT the gameobject
                if (hit.collider.gameObject != heldObject)
                {
                   StartCoroutine(TelepathyEnd(heldObject, mousePos));
                   heldObject = null;
                }
            }
        }

        //
        if (heldObject != null)
          {
                //get the lerpDirection
                Vector2 lerpDirection = Vector2.Lerp(heldObject.transform.position, mousePos, .3f);

                //use it to calculate a velocity _> do I need to split it so it does it over .3f?

                //move the rigidbody of the object#
                heldObject.GetComponent<Rigidbody2D>().MovePosition(lerpDirection);
            //AddForce(lerpDirection, ForceMode2D.Impulse);
            Debug.DrawRay(transform.position, heldObject.transform.position - transform.position, Color.red);

        }
        
          
     }
    

    public IEnumerator TelepathyEnd(GameObject heldObj, Vector3 mousePos)
    {
        var distance = Mathf.Infinity;
       
        while (distance > 0.5f){
            Debug.Log(distance);
            var heading = heldObj.transform.position - mousePos;
            distance = heading.magnitude;
            Vector2 lerpDirection = Vector2.Lerp(heldObj.transform.position,
            mousePos, .3f);
            heldObject.GetComponent<Rigidbody2D>().MovePosition(lerpDirection);
            yield return null;
            
        }
           
        
        yield return null;


    }

    //FOR TESTING
    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TelepathyStart();
        }
        else if(Input.GetMouseButtonUp(0) && heldObject != null)
        {
            StartCoroutine(TelepathyEnd(heldObject,Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            heldObject = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            FreezeObject();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {

        }
    }
}
