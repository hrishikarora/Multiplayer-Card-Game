using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventActionData;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button quickMatchButton;
    [SerializeField] private GameObject waitingPanel;

    private void OnEnable()
    {
        EventManager.AddListener<EventActionData.OnJoinedRoom>(OnJoinedRoom);
        EventManager.AddListener<EventActionData.ConnectedToMaster>(OnConnectedToMaster);
        statusText.text = "Connecting to server...";
        EventActionData.CreateJoinRoom createJoinRoom = new EventActionData.CreateJoinRoom();
        quickMatchButton.onClick.AddListener(() =>
        {
            EventManager.Trigger<EventActionData.CreateJoinRoom>(createJoinRoom);
            waitingPanel.SetActive(true);
        });
        quickMatchButton.interactable = false;

    }

    private void OnDisable()
    {
        EventManager.RemoveListener<EventActionData.OnJoinedRoom>(OnJoinedRoom);
        EventManager.RemoveListener<EventActionData.ConnectedToMaster>(OnConnectedToMaster);

        quickMatchButton.onClick.RemoveAllListeners();
    }

    private void OnConnectedToMaster(ConnectedToMaster master)
    {
        statusText.text = "Connected";

        quickMatchButton.interactable = true;

    }

    private void OnJoinedRoom(EventActionData.OnJoinedRoom onJoinedRoom)
    {
        statusText.text = "Waiting for opponent...";
        quickMatchButton.interactable = false;

        if (waitingPanel != null)
            waitingPanel.SetActive(true);
    }

  
}