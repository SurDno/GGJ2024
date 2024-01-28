using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class TestNetworkManager  : NetworkManager {
    public static TestNetworkManager i;

    public GameObject playerOnePrefab, playerTwoPrefab;

    private void Start() {
        base.Start();
        i = this;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        GameObject player = Instantiate(playerOnePrefab, new Vector2(20, -5f), Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);

        player.GetComponent<PlayerGraphics>().SetSecondPlayer(numPlayers == 1);

   }

   public override void OnServerDisconnect(NetworkConnectionToClient conn) {

        base.OnServerDisconnect(conn);
    }
}