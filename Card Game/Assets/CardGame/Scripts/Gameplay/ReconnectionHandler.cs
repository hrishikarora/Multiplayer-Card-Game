using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class ReconnectionHandler : MonoBehaviourPunCallbacks
{
    private int reconnectAttempts = 5;
    private int currentAttempt = 0;
    private bool isReconnecting = false;
    private bool isConnected = true;
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (GameflowManager.Instance.IsGameEnd) return;
        Debug.LogWarning($"Disconnected: {cause}");
        isConnected = false;
        if (!isReconnecting)
            StartCoroutine(AutoReconnect());
        EventManager.Trigger<EventActionData.OnPlayerDisconnected>(new EventActionData.OnPlayerDisconnected());

    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (GameflowManager.Instance.IsGameEnd) return;

        base.OnPlayerLeftRoom(otherPlayer);
        EventManager.Trigger<EventActionData.OnPlayerDisconnected>(new EventActionData.OnPlayerDisconnected());
    }

    private IEnumerator AutoReconnect()
    {
        isReconnecting = true;

        while (!PhotonNetwork.IsConnected && currentAttempt < reconnectAttempts)
        {
            Debug.LogWarning($"Reconnect attempt {currentAttempt + 1}/{reconnectAttempts}");
            currentAttempt++;
            PhotonNetwork.ConnectUsingSettings();
            yield return new WaitForSeconds(3f);
        }

        isReconnecting = false;
        currentAttempt = 0;

        if (!PhotonNetwork.IsConnected && !isConnected)
        {
            Debug.LogWarning("Failed to reconnect. Sending player to Lobby.");
            SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnected) return;
        isConnected = true;

        Debug.Log("Reconnected to Master successfully!");   

        if (isReconnecting)
        {
            if (!string.IsNullOrEmpty(GameManager.Instance.RoomName))
            {
                PhotonNetwork.RejoinRoom(GameManager.Instance.RoomName);
            }
            else
            {
                SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
            }

            isReconnecting = false;
        }
    }
    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log("Room rejoineed");
        EventManager.Trigger<EventActionData.OnPlayerReconnected>(new EventActionData.OnPlayerReconnected());
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
    }

}
