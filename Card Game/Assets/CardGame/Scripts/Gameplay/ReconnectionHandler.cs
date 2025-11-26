using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class ReconnectionHandler : MonoBehaviourPunCallbacks
{
    private int reconnectAttempts = 5;
    private int currentAttempt = 0;
    private bool isReconnecting = false;

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected: {cause}");

        if (!isReconnecting)
            StartCoroutine(AutoReconnect());
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

        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Failed to reconnect. Sending player to Lobby.");
            SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Reconnected to Master successfully!");

        if (isReconnecting)
        {
            if (!PhotonNetwork.InRoom && !string.IsNullOrEmpty(PhotonNetwork.CurrentRoom?.Name))
            {
                PhotonNetwork.RejoinRoom(PhotonNetwork.CurrentRoom.Name);
            }

            isReconnecting = false;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SceneLoader.Instance.LoadScene(GameConstants.LOBBY_SCENE);
    }

}
