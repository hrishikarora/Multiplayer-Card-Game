using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventActionData;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Button _quickMatchButton;
    [SerializeField] private GameObject _waitingPanel;

    private void OnEnable()
    {
        EventManager.AddListener<EventActionData.OnJoinedRoom>(OnJoinedRoom);
        EventManager.AddListener<EventActionData.ConnectedToMaster>(OnConnectedToMaster);
        _statusText.text = "Connecting to server...";
        EventActionData.CreateJoinRoom createJoinRoom = new EventActionData.CreateJoinRoom();
        _quickMatchButton.onClick.AddListener(() =>
        {
            EventManager.Trigger<EventActionData.CreateJoinRoom>(createJoinRoom);
            _waitingPanel.SetActive(true);
        });
        _quickMatchButton.interactable = false;

    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventActionData.OnJoinedRoom>(OnJoinedRoom);
        EventManager.RemoveListener<EventActionData.ConnectedToMaster>(OnConnectedToMaster);

        _quickMatchButton.onClick.RemoveAllListeners();
    }

    private void OnConnectedToMaster(ConnectedToMaster master)
    {
        _statusText.text = "Connected";

        _quickMatchButton.interactable = true;

    }

    private void OnJoinedRoom(EventActionData.OnJoinedRoom onJoinedRoom)
    {
        _statusText.text = "Waiting for opponent...";
        _quickMatchButton.interactable = false;

        if (_waitingPanel != null)
            _waitingPanel.SetActive(true);
    }

  
}