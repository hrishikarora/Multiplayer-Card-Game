using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : BaseSingleton<TurnManager>
{

    /// <summary>
    /// Cards played this turn by each player (key: "P1"/"P2").
    /// </summary>
    public static Dictionary<string, List<CardData>> PlayerPlayedCards { get; } = new();

    [Header("UI")]
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _placeCardsButton;

    private int currentCost = 1;
    private readonly List<CardUI> _selectedCards = new();
    private List<CardUI> _tempSelectedCards = new();
    private float _timerDuration = 30f;
    private float _timeLeft = 30f;
    private bool _timerRunning = false;
    private bool _hasEndedTurn;
    private string _myPlayerId;

    public bool HasEndedTurn => _hasEndedTurn;
    public bool AnyCardPlaced => _selectedCards.Count > 0;

    #region Unity Lifecycle

    private void Start()
    {
        InitPlayerIdAndEntries();
    }

    private void OnEnable()
    {
        EventManager.AddListener<EventActionData.TurnStart>(OnTurnStart);

        if (_endTurnButton != null)
            _endTurnButton.onClick.AddListener(OnEndTurnClicked);

        if (_placeCardsButton != null)
            _placeCardsButton.onClick.AddListener(OnPlaceCards);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventActionData.TurnStart>(OnTurnStart);

        if (_endTurnButton != null)
            _endTurnButton.onClick.RemoveListener(OnEndTurnClicked);

        if (_placeCardsButton != null)
            _placeCardsButton.onClick.RemoveListener(OnPlaceCards);
    }

    private void Update()
    {
        if (!_timerRunning) return;

        _timeLeft -= Time.deltaTime;

        if (_timeLeft <= 0)
        {
            _timerRunning = false;
            OnEndTurnClicked();
        }
    }
    #endregion

    #region Initialization

    private void InitPlayerIdAndEntries()
    {
        // Master client is P1, others are P2
        _myPlayerId = GameManager.Instance.CurrentPlayerID;

        PlayerEntry("P1");
        PlayerEntry("P2");
    }

    private void PlayerEntry(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("[TurnManager] EnsurePlayerEntry called with null/empty playerId.");
            return;
        }

        if (!PlayerPlayedCards.ContainsKey(playerId))
        {
            PlayerPlayedCards[playerId] = new List<CardData>();
        }
    }

    #endregion

    #region Event Handlers

    private void OnTurnStart(EventActionData.TurnStart e)
    {
        if (string.IsNullOrEmpty(_myPlayerId))
        {
            Debug.LogWarning("[TurnManager] myPlayerId was null on TurnStart; initializing now.");
            InitPlayerIdAndEntries();
        }

        currentCost = e.availableCost;
        _selectedCards.Clear();
        _tempSelectedCards.Clear();
        _hasEndedTurn = false;

        if (_endTurnButton != null)
            _endTurnButton.interactable = true;

        PlayerEntry(_myPlayerId);
        PlayerPlayedCards[_myPlayerId].Clear();
        currentCost = e.availableCost;
        _selectedCards.Clear();
        _tempSelectedCards.Clear();
        _hasEndedTurn = false;
        _timeLeft = _timerDuration;
        _timerRunning = true;

        if (_endTurnButton != null)
            _endTurnButton.interactable = true;
        Debug.Log($"[TurnManager] Turn {e.turnNumber} started. Available cost = {e.availableCost}");
    }

    #endregion

    #region Card Selection

    public void ToggleCardSelection(CardUI cardUI)
    {
        if (_hasEndedTurn)
            return;

        if (cardUI == null || cardUI.CardData == null)
            return;

        if (_tempSelectedCards.Contains(cardUI))
        {
            _tempSelectedCards.Remove(cardUI);
            cardUI.SetSelected(false);
            return;
        }

        int totalCost = _selectedCards.Sum(c => c.CardData.cost) + cardUI.CardData.cost + _tempSelectedCards.Sum(c=>c.CardData.cost);
        if (totalCost <= currentCost)
        {
            _tempSelectedCards.Add(cardUI);
            cardUI.SetSelected(true);
        }
        else
        {
            Debug.Log("[TurnManager] Cannot select card: not enough cost.");
        }
    }

    private void OnPlaceCards()
    {
        if (_hasEndedTurn)
            return;
        if(_tempSelectedCards.Count<=0)
        {
            Debug.LogError("No card selected");
            return;
        }
        _selectedCards.AddRange(_tempSelectedCards);

        HandManager.Instance.OnPlaceCardsUI(_tempSelectedCards);
        int totalCost = _selectedCards.Sum(c => c.CardData.cost);
        UIManager.Instance.SetCostUI(totalCost);

        foreach (var card in _tempSelectedCards)
        {
            HandManager.Instance.RemoveCard(card.gameObject);
            Destroy(card.gameObject);
        }

        _tempSelectedCards = new List<CardUI>();

    }


    #endregion

    #region End Turn

    private void OnEndTurnClicked()
    {
        if (_hasEndedTurn)
            return;

        _hasEndedTurn = true;

        if (_endTurnButton != null)
            _endTurnButton.interactable = false;

        PlayerEntry(_myPlayerId);
        PlayerPlayedCards[_myPlayerId].Clear();

        foreach (CardUI cardUI in _selectedCards)
        {
            PlayerPlayedCards[_myPlayerId].Add(cardUI.CardData);
        }

        int[] playedIds = _selectedCards.Select(c => c.CardData.id).ToArray();

        var msg = new EndTurnMessage
        {
            action = GameConstants.END_TURN_ACTION_NAME,
            playerId = _myPlayerId
        };

        string json = JsonUtility.ToJson(msg);
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(GameConstants.GAME_EVENT_CODE, json, options, SendOptions.SendReliable);

        Debug.Log($"[TurnManager] {_myPlayerId} ended turn with cards: {string.Join(",", playedIds)}");
    }

    #endregion

    public void PauseGame(bool pause)
    {
        _timerRunning = !pause;
    }
    public static void ResetPlayedCards()
    {
        PlayerPlayedCards.Clear();
        PlayerPlayedCards["P1"] = new List<CardData>();
        PlayerPlayedCards["P2"] = new List<CardData>();
    }
}
