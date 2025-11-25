using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventActionData;

public class UIManager : BaseSingleton<UIManager>
{
    [SerializeField] private GameObject _endGameScreen;
    [SerializeField] private TextMeshProUGUI _currentTurnText;
    [SerializeField] private TextMeshProUGUI _currentPlayerScore;
    [SerializeField] private TextMeshProUGUI _opponentScore;
    [SerializeField] private TextMeshProUGUI _deckCardCount;
    [SerializeField] private TextMeshProUGUI _noCardPlacedText;
    [SerializeField] private TextMeshProUGUI _currentPlayerID;
    [SerializeField] private TextMeshProUGUI _opponentPlayerID;

    [SerializeField] private List<Image> _costImages = new List<Image>();
    private void OnEnable()
    {
        EventManager.AddListener<EventActionData.TurnStart>(StartTurn);
        string currentPlayerID = GameManager.Instance.CurrentPlayerID == GameConstants.P1 ? "Player 1" : "Player 2";
        string opponentPlayerID = GameManager.Instance.CurrentPlayerID == GameConstants.P1 ? "Player 2" : "Player 1";
        _currentPlayerID.SetText(currentPlayerID);
        _opponentPlayerID.SetText(opponentPlayerID);

    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventActionData.TurnStart>(StartTurn);

    }

    public void SetScore(string playerID, int score)
    {
        TextMeshProUGUI scoreText = playerID == GameManager.Instance.CurrentPlayerID ? _currentPlayerScore : _opponentScore;
        scoreText.text ="score: " + score.ToString();
    }

    private void StartTurn(TurnStart start)
    {
        _currentTurnText.SetText($"TURN {start.turnNumber} / {GameConstants.TOTAL_TURNS}");
        ResetCostUI();
    }

    public void UpdateDeckCardUI(int count)
    {
        _deckCardCount.SetText($"Deck: {count} cards left");
    }

    public void SetPlacedCardUI()
    {
        _noCardPlacedText.gameObject.SetActive(TurnManager.Instance.AnyCardPlaced ? false : true);
    }

    public void SetCostUI(int cost)
    {
       for(int i = 0; i < cost;i++)
        {
            _costImages[i].color = Color.red;
        }
    }

    public void ResetCostUI()
    {
        for(int i = 0; i < _costImages.Count;i++)
        {
            _costImages[i].color = Color.white;
        }
    }

    public void OpenEndGameScreen()
    {
        _endGameScreen.SetActive(true);
    }
}
