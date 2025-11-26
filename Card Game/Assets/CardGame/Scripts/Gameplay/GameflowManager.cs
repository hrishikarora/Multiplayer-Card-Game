using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EventActionData;

public class GameflowManager : BaseSingleton<GameflowManager>
{

    private int _currentTurn;
    private int _playersEnded;
    public int CurrentTurn=>_currentTurn;

    private Coroutine disconnectTimerRoutine;
    private const int DisconnectTimeout = 60;

    public bool IsGameEnd = false;
    private void OnEnable()
    {
        EventManager.AddListener<EventActionData.GameStart>(OnGameStart);
        EventManager.AddListener<EventActionData.PlayerEndedTurn>(OnPlayerEndedTurn);
        EventManager.AddListener<EventActionData.SendRevealCards>(RevealCards);
        EventManager.AddListener<EventActionData.OnPlayerDisconnected>(PlayerDisconnected);
        EventManager.AddListener<EventActionData.OnPlayerReconnected>(OnPlayerReconnected);
        IsGameEnd = false;

    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventActionData.GameStart>(OnGameStart);
        EventManager.RemoveListener<EventActionData.PlayerEndedTurn>(OnPlayerEndedTurn);
        EventManager.RemoveListener<EventActionData.SendRevealCards>(RevealCards);
        EventManager.RemoveListener<EventActionData.OnPlayerDisconnected>(PlayerDisconnected);
        EventManager.RemoveListener<EventActionData.OnPlayerReconnected>(OnPlayerReconnected);
    }


    private void OnGameStart(EventActionData.GameStart gameStart)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        _currentTurn = 1;
        _playersEnded = 0;
        StartTurn(_currentTurn);
    }

    private void StartTurn(int turnNum)
    {
        var msg = new TurnStartMessage
        {
            action = GameConstants.TURN_START_ACTION_NAME,
            turnNumber = turnNum,
            availableCost = turnNum
        };

        var json = JsonUtility.ToJson(msg);
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(GameConstants.GAME_EVENT_CODE, json, options, SendOptions.SendReliable);

        Debug.Log($"[GameFlowController] Turn {turnNum} started. Cost = {msg.availableCost}");
    }

    private void OnPlayerEndedTurn(EventActionData.PlayerEndedTurn e)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        _playersEnded++;
        Debug.Log($"[GameFlowController] Player {e.playerId} ended turn. Total ended: {_playersEnded}");

        if (_playersEnded >= GameConstants.MAX_PLAYERS)
        {
            _playersEnded = 0;
            RequestCards();
            Invoke(nameof(NextTurn), 3f); // 3 sec reveal window
        }
    }

    private void RequestCards()
    {
  
        var msg = new RequestCardMessage
        {
            action = GameConstants.REQ_CARD_ACTION_NAME,
            playerId = GameManager.Instance.CurrentPlayerID
        };

        var json = JsonUtility.ToJson(msg);
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(GameConstants.GAME_EVENT_CODE, json, options, SendOptions.SendReliable);
    }

    private void RevealCards(SendRevealCards sendRevealCards)
    {
        var p1Ids = TurnManager.PlayerPlayedCards.TryGetValue(sendRevealCards.PlayerId, out var p1Cards)
            ? p1Cards.Select(c => c.id).ToArray()
            : System.Array.Empty<int>();

 
        SendRevealForPlayer(sendRevealCards.PlayerId, p1Ids);

        Debug.Log($"[GameFlowController] RevealCards {sendRevealCards.PlayerId}: {string.Join(",", p1Ids)}");
    }

    private void NextTurn()
    {
        _currentTurn++;

        if (_currentTurn > 6)
        {
            EndGame();
        }
        else
        {
            StartTurn(_currentTurn);
        }
    }

    private void SendRevealForPlayer(string playerId, int[] cardIds)
    {
        var msg = new RevealCardsMessage
        {
            action = GameConstants.REVEAL_CARDS_ACTION_NAME,
            playerId = playerId,
            cardIds = cardIds
        };

        var json = JsonUtility.ToJson(msg);
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(GameConstants.GAME_EVENT_CODE, json, options, SendOptions.SendReliable);
    }

    private void EndGame()
    {
        IsGameEnd = true;
        int p1Score = GameManager.Instance.PlayerScore[GameConstants.P1];
        int p2Score = GameManager.Instance.PlayerScore[GameConstants.P2];
        string winner = p1Score > p2Score ? GameConstants.P1 : GameConstants.P2;
        var msg = new GameEndMessage
        {
            action = GameConstants.END_GAME_ACTION_NAME,
            winner = winner,
            p1Score = p1Score,
            p2Score = p2Score
        };

        var json = JsonUtility.ToJson(msg);
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(GameConstants.GAME_EVENT_CODE, json, options, SendOptions.SendReliable);

        Debug.Log("[GameFlowController] Game ended.");
    }


    private void PlayerDisconnected(OnPlayerDisconnected disconnected)
    {
        Debug.Log("[GameFlow] Player disconnected. Starting timeout...");

        TurnManager.Instance.PauseGame(true);

        if (PhotonNetwork.IsMasterClient)
        {
            if (disconnectTimerRoutine != null)
                StopCoroutine(disconnectTimerRoutine);

            disconnectTimerRoutine = StartCoroutine(DisconnectTimeoutRoutine());
        }
    }


    private void OnPlayerReconnected(OnPlayerReconnected onPlayerReconnected)
    {
        if (disconnectTimerRoutine != null)
        {
            StopCoroutine(disconnectTimerRoutine);
            disconnectTimerRoutine = null;
        }
        _currentTurn--;
        TurnManager.Instance.PauseGame(false);
        NextTurn();
    }

    private IEnumerator DisconnectTimeoutRoutine()
    {
        int timeLeft = DisconnectTimeout;

        while (timeLeft > 0)
        {

            yield return new WaitForSeconds(1f);

            timeLeft--;
        }
        IsGameEnd = true;
        Debug.Log("[GameFlow] Timeout reached. Ending game...");
        var msg = new GameEndMessage
        {
            action = GameConstants.END_GAME_ACTION_NAME,
            winner = GameManager.Instance.CurrentPlayerID,
            p1Score = GameManager.Instance.PlayerScore[GameConstants.P1],
            p2Score = GameManager.Instance.PlayerScore[GameConstants.P2]
        };

        var json = JsonUtility.ToJson(msg);
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(GameConstants.GAME_EVENT_CODE, json, options, SendOptions.SendReliable);
    }
}
