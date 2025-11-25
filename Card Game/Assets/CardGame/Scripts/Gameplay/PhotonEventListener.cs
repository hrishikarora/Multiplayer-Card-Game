using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonEventListener : MonoBehaviour, IOnEventCallback
{
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code != GameConstants.GAME_EVENT_CODE)
            return;

        if (photonEvent.CustomData is not string json)
        {
            Debug.LogWarning("[PhotonEventListener] Received event with non-string payload.");
            return;
        }

        Debug.Log($"[PhotonEventListener] Received JSON: {json}");

        var wrapper = JsonUtility.FromJson<ActionWrapper>(json);
        if (wrapper == null || string.IsNullOrEmpty(wrapper.action))
        {
            Debug.LogWarning("[PhotonEventListener] Invalid action wrapper.");
            return;
        }

        switch (wrapper.action)
        {
            case GameConstants.GAME_START_ACTION_NAME:
                var gameStart = JsonUtility.FromJson<GameStartJson>(json);
                EventManager.Trigger(new EventActionData.GameStart
                {
                    playerIds = gameStart.playerIds,
                    totalTurns = gameStart.totalTurns
                });
                Debug.Log("Game start raised");
                break;

            default:
                Debug.LogWarning($"[PhotonEventListener] Unknown action: {wrapper.action}");
                break;
        }
    }

    [System.Serializable]
    private class ActionWrapper
    {
        public string action;
    }

    [System.Serializable] private class GameStartJson { public string action; public string[] playerIds; public int totalTurns; }
    [System.Serializable] private class TurnStartJson { public string action; public int turnNumber; public int availableCost; }
    [System.Serializable] private class EndTurnJson { public string action; public string playerId; }
    [System.Serializable] private class RevealCardsJson { public string action; public int[] cardIds; public string playerId; }
    [System.Serializable] private class GameEndJson { public string action; public string winner; public int p1Score; public int p2Score; }
}
