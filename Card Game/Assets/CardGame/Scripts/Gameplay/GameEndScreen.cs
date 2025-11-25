using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerWonText;
    [SerializeField] private TextMeshProUGUI _playerOneScore;
    [SerializeField] private TextMeshProUGUI _playerTwoScore;
    [SerializeField] private Button _closeBtn;

    private void OnEnable()
    {
        PhotonNetwork.Disconnect();

        _closeBtn.onClick.AddListener(OnClose);
        EventManager.AddListener<EventActionData.GameEnd>(UpdateEndGameScreen);
    }

    private void OnDisable()
    {
        _closeBtn.onClick.RemoveListener(OnClose);
        EventManager.RemoveListener<EventActionData.GameEnd>(UpdateEndGameScreen);

    }


    private void UpdateEndGameScreen(EventActionData.GameEnd endGame)
    {
        string wonText = $"{endGame.winner} Won!";
        _playerWonText.SetText(wonText);
        _playerOneScore.SetText($"{endGame.p1Score}");
        _playerTwoScore.SetText($"{endGame.p2Score}");

    }


    private void OnClose()
    {
        SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
    }
}
