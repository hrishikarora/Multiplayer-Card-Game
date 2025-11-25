using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (PhotonNetwork.CurrentRoom == null ||
            PhotonNetwork.CurrentRoom.PlayerCount < GameConstants.MAX_PLAYERS)
        {
            Debug.Log("[GameManager] Waiting for both players to join before starting game.");
            return;
        }

        var msg = new GameStartMessage
        {
            action = GameConstants.GAME_START_ACTION_NAME,
            playerIds = new[] { GameConstants.P1, GameConstants.P2 },
            totalTurns = GameConstants.TOTAL_TURNS,
        };

        var json = JsonUtility.ToJson(msg);
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(GameConstants.GAME_EVENT_CODE, json, options, SendOptions.SendReliable);

        Debug.Log("[GameManager] GameStart raised.");
    }

    [System.Serializable]
    private class GameStartMessage
    {
        public string action;
        public string[] playerIds;
        public int totalTurns;
    }
}
