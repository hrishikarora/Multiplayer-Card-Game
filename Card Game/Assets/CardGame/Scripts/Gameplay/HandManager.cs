using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using static EventActionData;

public class HandManager : BaseSingleton<HandManager>
{
    [SerializeField] private Transform _cardParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject _opponendCardPrefab;
    [SerializeField] private Transform _opponentCardParent;
    [SerializeField] private Transform _placedCardParent;

    private List<GameObject> currentCards = new List<GameObject>();
    private List<GameObject> _opponentCards = new List<GameObject>();

    private List<int> _oppoenentHandCards = new List<int>();

    private List<GameObject> _placedCardList = new List<GameObject>();


    public Dictionary<string,List<int>> playerHandCards = new Dictionary<string, List<int>>();
    public Dictionary<string, List<int>> placedCards = new Dictionary<string, List<int>>();


    private void OnEnable()
    {
        EventManager.AddListener<EventActionData.GameStart>(OnGameStart);
        EventManager.AddListener<EventActionData.TurnStart>(OnTurnStart);
        EventManager.AddListener<EventActionData.RevealCards>(OnRevealCards);
        EventManager.AddListener<EventActionData.GetPlayerCards>(AddPlayerCards);
        playerHandCards.Clear();
        playerHandCards.Add(GameConstants.P1, new List<int>());
        playerHandCards.Add(GameConstants.P2, new List<int>());
        placedCards.Clear();
        placedCards.Add(GameConstants.P1, new List<int>());
        placedCards.Add(GameConstants.P2, new List<int>());
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventActionData.GameStart>(OnGameStart);
        EventManager.RemoveListener<EventActionData.TurnStart>(OnTurnStart);
        EventManager.RemoveListener<EventActionData.RevealCards>(OnRevealCards);
        EventManager.RemoveListener<EventActionData.GetPlayerCards>(AddPlayerCards);

    }

    private void AddPlayerCards(GetPlayerCards cards)
    {
        if(cards.playerId==GameManager.Instance.CurrentPlayerID)
        {
            return;
        }

        playerHandCards[cards.playerId].AddRange(cards.cardIds);
    }

    private void OnGameStart(EventActionData.GameStart e)
    {
        var hand = CardManager.Instance.DealStartingHand();
        DisplayHand(hand);
           
        List<int> cardIds = hand.Select(x=>x.id).ToList();
        playerHandCards[GameManager.Instance.CurrentPlayerID].AddRange(cardIds);
        var msg = new PlayerCardModified
        {
            action = GameConstants.PLAYER_CARDS_MODIFIELD,
            playerId = GameManager.Instance.CurrentPlayerID,
            handCardIDs = cardIds
        };

        string json = JsonUtility.ToJson(msg);
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(GameConstants.GAME_EVENT_CODE, json, options, SendOptions.SendReliable);
    }

    private void DisplayHand(List<CardData> cards)
    {
        ClearHand();
        foreach (var card in cards)
        {
            CreateCardUI(card);
        }
    }

    private void OnTurnStart(EventActionData.TurnStart e)
    {
        if (e.turnNumber > 1)
        {
            var newCard = CardManager.Instance.DrawCard();
            AddCardToHand(newCard);

            List<int> cardIds = new List<int>(newCard.id);
            playerHandCards[GameManager.Instance.CurrentPlayerID].AddRange(cardIds);
            var msg = new PlayerCardModified
            {
                action = GameConstants.PLAYER_CARDS_MODIFIELD,
                playerId = GameManager.Instance.CurrentPlayerID,
                handCardIDs = cardIds
            };

            string json = JsonUtility.ToJson(msg);
            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        }
    }

    private void AddCardToHand(CardData card)
    {
        CreateCardUI(card);
    }

    private void CreateCardUI(CardData card)
    {
        GameObject cardGO = Instantiate(cardPrefab, _cardParent);
        currentCards.Add(cardGO);

        CardUI cardUI = cardGO.GetComponent<CardUI>();
        cardUI.Initialize(card);
    }
    private void ClearHand()
    {
        foreach (var card in currentCards)
            Destroy(card.gameObject);
        currentCards.Clear();
    }

    public void RemoveCard(GameObject card)
    {
        currentCards.Remove(card);
    }

    private void OnRevealCards(RevealCards cards)
    {   
        if(cards.playerId==GameManager.Instance.CurrentPlayerID)
        {
            Debug.LogWarning($"[Reveal Cards] Don't reveal current player cards");
            return;
        }

        foreach (var card in cards.cardIds)
        {
            CardData cardData = CardManager.Instance.AllCards.FirstOrDefault(x => x.id == card);
            if (cardData == null)   
            {
                Debug.LogError($"[Reveal Cards] card not found");
                continue;
            }
            placedCards[cards.playerId].Add(card);

            GameObject cardGO = Instantiate(_opponendCardPrefab, _opponentCardParent);
            _opponentCards.Add(cardGO);

            CardUI cardUI = cardGO.GetComponent<CardUI>();
            cardUI.Initialize(cardData,false);
        }
    }

    public void OnPlaceCardsUI(List<CardUI> cards)
    {
        if(cards == null || cards.Count <=0)
        {
            Debug.LogError($"Empty card list");
            return;
        }

        foreach(var card in cards)
        {
            GameObject cardGO = Instantiate(_opponendCardPrefab, _placedCardParent);
            _placedCardList.Add(cardGO);
            placedCards[GameManager.Instance.CurrentPlayerID].Add(card.CardData.id);
            CardUI cardUI = cardGO.GetComponent<CardUI>();
            cardUI.Initialize(card.CardData, false);
        }

        UIManager.Instance.SetPlacedCardUI();
    }

    public List<int> GetCardIds(List<GameObject> cards)
    {
        List<int> cardIds = new List<int>();

        foreach (var card in cards)
        {
            cardIds.Add(card.GetComponent<CardUI>().CardData.id);
        }
        return cardIds;
    }
}
