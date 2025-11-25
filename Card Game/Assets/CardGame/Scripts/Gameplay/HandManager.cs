using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    private List<GameObject> _placedCardList = new List<GameObject>();

    private void OnEnable()
    {
        EventManager.AddListener<EventActionData.GameStart>(OnGameStart);
        EventManager.AddListener<EventActionData.TurnStart>(OnTurnStart);
        EventManager.AddListener<EventActionData.RevealCards>(OnRevealCards);

    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventActionData.GameStart>(OnGameStart);
        EventManager.AddListener<EventActionData.TurnStart>(OnTurnStart);
        EventManager.AddListener<EventActionData.RevealCards>(OnRevealCards);

    }


    private void OnGameStart(EventActionData.GameStart e)
    {
        var hand = CardManager.Instance.DealStartingHand();
        DisplayHand(hand);
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

            CardUI cardUI = cardGO.GetComponent<CardUI>();
            cardUI.Initialize(card.CardData, false);
        }

        UIManager.Instance.SetPlacedCardUI();
    }
}
