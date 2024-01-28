using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    public static CheckPoints Instance;
    //make this a singleton so players can get instance of it?
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public Transform[] checkPoints;
    public Transform[] cameraLevelPositions;
    public int checkPointIndex; //where the respawn point is!
    public int cameraLevelPositionIndex;

    public void UpdateCheckPoint()
    {
        checkPoints[checkPointIndex].gameObject.GetComponent<Collider2D>().enabled = false; //disable trigger
        checkPointIndex++;

    }

    public Vector2 GetCurrentCheckPoint()
    {
        return checkPoints[checkPointIndex].position;
    }

    public void ChangeCameraPosition()
    {
        cameraLevelPositionIndex++;
        Camera.main.transform.position = cameraLevelPositions[cameraLevelPositionIndex].position;
    }

}
