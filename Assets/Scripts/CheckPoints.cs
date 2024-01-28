using Mirror;
using UnityEngine;

public class CheckPoints : NetworkBehaviour {
    [SyncVar] public Vector3 lastCheckpointPos;
    public static Vector3 lastCheckpointPosShared;
    [SerializeField] private bool nextLevel;
    [SerializeField] private Transform newCamPos;

    public Transform[] cameraLevelPositions;
    public int cameraLevelPositionIndex;

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision) {
        lastCheckpointPosShared = transform.position;

        if (nextLevel)
            Camera.main.transform.position = newCamPos.position;
    }

    private void Update() {
        if (isServer)
            lastCheckpointPos = lastCheckpointPosShared;
        else
            lastCheckpointPosShared = lastCheckpointPos;
    }
}
