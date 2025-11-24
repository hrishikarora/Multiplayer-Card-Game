using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventData;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button quickMatchButton;
    [SerializeField] private GameObject waitingPanel;

    private void OnEnable()
    {
        EventManager.AddListener<EventData.OnJoinedRoom>(OnJoinedRoom);
        EventManager.AddListener<EventData.ConnectedToMaster>(OnConnectedToMaster);
        statusText.text = "Connecting to server...";
        EventData.CreateJoinRoom createJoinRoom = new EventData.CreateJoinRoom();
        quickMatchButton.onClick.AddListener(() =>
        {
            EventManager.Trigger<EventData.CreateJoinRoom>(createJoinRoom);
            waitingPanel.SetActive(true);
        });
        quickMatchButton.interactable = false;

    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventData.OnJoinedRoom>(OnJoinedRoom);
        EventManager.RemoveListener<EventData.ConnectedToMaster>(OnConnectedToMaster);

        quickMatchButton.onClick.RemoveAllListeners();
    }

    private void OnConnectedToMaster(ConnectedToMaster master)
    {
        statusText.text = "Connected";

        quickMatchButton.interactable = true;

    }

    private void OnJoinedRoom(EventData.OnJoinedRoom onJoinedRoom)
    {
        statusText.text = "Waiting for opponent...";
        quickMatchButton.interactable = false;

        if (waitingPanel != null)
            waitingPanel.SetActive(true);
    }

  
}