using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{//NEEDS TO INHERIT FROM PlayerCommon
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
        GameObject freezeProjectile = Instantiate(iceProjectilePrefab, transform);
        var direction =(transform.position - mousePos).normalized;
        freezeProjectile.transform.parent = null;
       // freezeProjectile.GetComponent<FreezeProjectile>().moveDir = direction;

    }

    //Ability 3: Telepathy
    public void TelepathyStart()
    {


        //do a raycast from transform to mousePosition
        var camera = Camera.main;

        var screenRay = camera.ScreenPointToRay(Input.mousePosition);
        Vector3 selectedPoint = Vector3.zero;
        //Do 2 different types of raycasts
        //Find your heldObject -> Case: Don't have a held object yet
        if(heldObject == null)
        {
            if (Physics.Raycast(screenRay, out RaycastHit hit))
            {
                selectedPoint = hit.point;
                if (hit.collider != null)
                {
                    //if it isn't colliding with the held object then 
                    if (heldObject != null && hit.collider.gameObject != heldObject)
                    {
                        //Drop the object

                    }
                    else
                    {
                        heldObject = hit.collider.gameObject;
                    }

                }
            }
        }
        //2nd raycast is from player to heldObject -> Case: Object is behind wall so drop it
        else
        {
            screenRay = camera.ScreenPointToRay(heldObject.transform.position);
            if (Physics.Raycast(screenRay,out RaycastHit hit))
            {
                if(hit.collider != null)
                {
                    //if it collides with something that ISNT the gameobject
                    if(hit.collider.gameObject != heldObject)
                    {
                        //TelepathyEnd();
                    }
                }
        }
        
        if(heldObject != null)
        {
            //



            heldObject.transform.position = Vector2.Lerp(heldObject.transform.position, camera.ScreenToWorldPoint(Input.mousePosition), Time.deltaTime);
           //heldObject.transform.position = (Vector2)camera.ScreenToWorldPoint(Input.mousePosition);
            
        }
        //var originToSelectionRay = new Ray(transform.position, selectedPoint - transform.position);
        Debug.DrawRay(transform.position, heldObject.transform.position - transform.position, Color.red);
    }

    public IEnumerator TelepathyEnd(GameObject heldObj, Vector3 mousePos)
    {
        var distance = Mathf.Infinity;
       
        while (distance > 0.5f){
            Debug.Log(distance);
            var heading = heldObj.transform.position - mousePos;
            distance = heading.magnitude;
            heldObj.transform.position = Vector2.Lerp(heldObj.transform.position,
              mousePos, .3f);
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
