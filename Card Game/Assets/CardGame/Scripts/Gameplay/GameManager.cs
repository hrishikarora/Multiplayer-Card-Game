using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager>
{
    public string CurrentPlayerID => PhotonNetwork.IsMasterClient ? GameConstants.P1 : GameConstants.P2;
    private readonly Dictionary<string, int> playerScore = new Dictionary<string, int>();

    private GameSnapshot _gameSnapshot = new GameSnapshot();
    public Dictionary<string, int> PlayerScore => playerScore;

    public string RoomName = "";
    private void OnEnable()
    {
        playerScore[GameConstants.P1] = 0;
        playerScore[GameConstants.P2] = 0;
        EventManager.AddListener<EventActionData.RevealCards>(OnCardsReveal);
        _gameSnapshot = new GameSnapshot();
        RoomName = PhotonNetwork.CurrentRoom?.Name;
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventActionData.RevealCards>(OnCardsReveal);
        EventManager.ClearAll();
    }

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
    private void OnCardsReveal(EventActionData.RevealCards cards)
    {
        playerScore.TryGetValue(cards.playerId, out var myScore);

        string opponentId = cards.playerId == GameConstants.P1 ? GameConstants.P2 : GameConstants.P1;
        playerScore.TryGetValue(opponentId, out var oppScore);

        foreach (var cardId in cards.cardIds)
        {
            CardData card = CardManager.Instance.AllCards.FirstOrDefault(x => x.id == cardId);
            if (card == null)
                continue;

            myScore += card.power;

            if (card.ability != null && !string.IsNullOrEmpty(card.ability.type))
            {
                switch (card.ability.type)
                {
                    case "GainPoints":
                        myScore += card.ability.value;
                        break;

                    case "StealPoints":
                        int stealAmount = Mathf.Clamp(card.ability.value, 0, oppScore);
                        oppScore -= stealAmount;
                        myScore += stealAmount;
                        break;

                }
            }
        }

        playerScore[cards.playerId] = myScore;
        playerScore[opponentId] = oppScore;

        UIManager.Instance.SetScore(cards.playerId, myScore);
        UIManager.Instance.SetScore(opponentId, oppScore);

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            var props = new ExitGames.Client.Photon.Hashtable
            {
                { "p1Score", playerScore.TryGetValue(GameConstants.P1, out var p1) ? p1 : 0 },
                { "p2Score", playerScore.TryGetValue(GameConstants.P2, out var p2) ? p2 : 0 }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            TakeSnapshot();
        }
    }

    private void TakeSnapshot()
    {
        int currentTurn = GameflowManager.Instance.CurrentTurn;
        playerScore.TryGetValue(GameConstants.P1, out var p1Score);
        playerScore.TryGetValue(GameConstants.P2, out var p2Score);


        List<int> playerOneHandCards = HandManager.Instance.playerHandCards[GameConstants.P1];
        List<int> playerTwoHandCards = HandManager.Instance.playerHandCards[GameConstants.P2];
        List<int> playerOnePlacedCards = HandManager.Instance.placedCards[GameConstants.P1];
        List<int> playerTwoPlaceCards = HandManager.Instance.placedCards[GameConstants.P2];
    }
}
