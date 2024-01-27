using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeManager : MonoBehaviour
{
    public static FreezeManager instance;
    public List<GameObject> frozenObjects = new List<GameObject>();
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public IEnumerator FrozenObjectCountdown(GameObject collidedObject)
    {
        frozenObjects.Add(collidedObject);
        collidedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(5f);
        collidedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        frozenObjects.Remove(collidedObject);

        yield return null;

    }

    public void StartFrozenObjectCountdown(GameObject collidedObject)
    {
        StartCoroutine(FrozenObjectCountdown(collidedObject));
    }

}
