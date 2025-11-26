using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class ReconnectionHandler : MonoBehaviourPunCallbacks
{
    private int reconnectAttempts = 7;
    private int currentAttempt = 0;
    private bool isReconnecting = false;
    private bool isConnected = true;
    private bool wasConnectedBefore = true;

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (GameflowManager.Instance.IsGameEnd) return;

        Debug.LogWarning($"Disconnected: {cause}");

        wasConnectedBefore = false;

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
        currentAttempt = 0;

        while (!PhotonNetwork.IsConnectedAndReady && currentAttempt < reconnectAttempts)
        {
            currentAttempt++;
            Debug.LogWarning($"Reconnect attempt {currentAttempt}/{reconnectAttempts}");

            PhotonNetwork.ConnectUsingSettings();

            yield return new WaitForSeconds(3f);
        }

        isReconnecting = false;

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Failed to reconnect → Sending to lobby.");
            SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
        }
    }

    public override void OnConnectedToMaster()
    {
        if (wasConnectedBefore) return;

        Debug.Log("Reconnected to Master successfully!");

        wasConnectedBefore = true;

        if (!string.IsNullOrEmpty(GameManager.Instance.RoomName))
        {
            PhotonNetwork.RejoinRoom(GameManager.Instance.RoomName);
        }
        else
        {
            SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
            Debug.Log("Expected player rejoined");
            EventManager.Trigger(new EventActionData.OnPlayerReconnected());
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("[Rejoin]: " + message);
        SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
    }

}
