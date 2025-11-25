using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Handles Photon connection
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string _roomName = "CardGame";


    public override void OnEnable()
    {
        base.OnEnable();
        EventManager.AddListener<EventActionData.CreateJoinRoom>(JoinOrCreateRoom);
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public override void OnDisable()
    {
        base.OnDisable();
        EventManager.RemoveListener<EventActionData.CreateJoinRoom>(JoinOrCreateRoom);
    }
    private void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");

        EventManager.Trigger(new EventActionData.ConnectedToMaster());
    }

    public void JoinOrCreateRoom(EventActionData.CreateJoinRoom createJoinRoom)
    {
        PhotonNetwork.JoinOrCreateRoom(
              _roomName,
              new RoomOptions { MaxPlayers = maxPlayers },
              TypedLobby.Default
          );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.PlayerCount}/2");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join room failed: {returnCode} - {message}");
        EventActionData.OnJoinedRoom joinRoom = new EventActionData.OnJoinedRoom();
        EventManager.Trigger<EventActionData.OnJoinedRoom>( joinRoom );
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers && PhotonNetwork.IsMasterClient)
        {
            SceneLoader.Instance.LoadScene(GameConstants.GAME_SCENE);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected from Photon: {cause}");
    }

    private byte maxPlayers => GameConstants.MAX_PLAYERS;

}
