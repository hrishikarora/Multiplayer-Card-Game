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
                var gameStart = JsonUtility.FromJson<GameStartMessage>(json);
                EventManager.Trigger(new EventActionData.GameStart
                {
                    playerIds = gameStart.playerIds,
                    totalTurns = gameStart.totalTurns
                });
                Debug.Log("Game start raised");
                break;

            case GameConstants.TURN_START_ACTION_NAME:
                var turnStart = JsonUtility.FromJson<TurnStartMessage>(json);
                EventManager.Trigger(new EventActionData.TurnStart
                {
                    turnNumber = turnStart.turnNumber,
                    availableCost = turnStart.availableCost
                });
                break;
            case GameConstants.END_TURN_ACTION_NAME:
                var endTurn = JsonUtility.FromJson<EndTurnMessage>(json);
                EventManager.Trigger(new EventActionData.PlayerEndedTurn
                {
                    playerId = endTurn.playerId
                });
                break;
            case GameConstants.REVEAL_CARDS_ACTION_NAME:
                var reveal = JsonUtility.FromJson<RevealCardsMessage>(json);
                EventManager.Trigger(new EventActionData.RevealCards
                {
                    cardIds = reveal.cardIds,
                    playerId = reveal.playerId
                });
                break;
            case GameConstants.REQ_CARD_ACTION_NAME:
                var req = JsonUtility.FromJson<RequestCardMessage>(json);
                EventManager.Trigger(new EventActionData.SendRevealCards()
                {
                    PlayerId = GameManager.Instance.CurrentPlayerID
                });
                break;
            case GameConstants.END_GAME_ACTION_NAME:
                var endGame = JsonUtility.FromJson<GameEndMessage>(json);
                UIManager.Instance.OpenEndGameScreen();
                EventManager.Trigger(new EventActionData.GameEnd()
                {
                    winner = endGame.winner,
                    p1Score = endGame.p1Score,
                    p2Score = endGame.p2Score
                });
                break;
            case GameConstants.PLAYER_CARDS_MODIFIELD:
                var playerCards = JsonUtility.FromJson<PlayerCardModified>(json);
                EventManager.Trigger(new EventActionData.GetPlayerCards()
                {
                    playerId = playerCards.playerId,
                    cardIds = playerCards.handCardIDs
                });
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

}
