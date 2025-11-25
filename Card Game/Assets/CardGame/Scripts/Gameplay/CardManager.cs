using System.Collections.Generic;
using UnityEngine;

public class CardManager : BaseSingleton<CardManager>
{
    private readonly List<CardData> _deck = new();
    private List<CardData> _allCards = new();

    protected override void Awake()
    {
        base.Awake();
        LoadAllCards();
        CreateShuffledDeck();
    }

    private void LoadAllCards()
    {
        var jsonFile = Resources.Load<TextAsset>("Cards/card");
        if (jsonFile == null)
        {
            Debug.LogError("[CardManager] card.json not found in Resources.");
            _allCards = new List<CardData>();
            return;
        }

        var dataList = JsonUtility.FromJson<CardDataList>(jsonFile.text);
        _allCards = new List<CardData>(dataList.cards);
    }

    private void CreateShuffledDeck()
    {
        _deck.Clear();
        _deck.AddRange(_allCards);
        _deck.Shuffle();
    }

    public CardData DrawCard()
    {
        if (_deck.Count == 0)
        {
            Debug.LogWarning("[CardManager] Deck empty. Reshuffling.");
            CreateShuffledDeck();
        }

        var card = _deck[0];
        _deck.RemoveAt(0);
        return card;
    }

    public List<CardData> DealStartingHand()
    {
        var hand = new List<CardData>(3);
        for (var i = 0; i < 3; i++)
        {
            hand.Add(DrawCard());
        }
        return hand;
    }
}
