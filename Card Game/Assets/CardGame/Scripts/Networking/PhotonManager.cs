using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Handles Photon connection
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string roomName = "CardGame";


    public override void OnEnable()
    {
        base.OnEnable();
        EventManager.AddListener<EventData.CreateJoinRoom>(JoinOrCreateRoom);
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public override void OnDisable()
    {
        base.OnDisable();
        EventManager.RemoveListener<EventData.CreateJoinRoom>(JoinOrCreateRoom);
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

        EventManager.Trigger(new EventData.ConnectedToMaster());
    }

    public void JoinOrCreateRoom(EventData.CreateJoinRoom createJoinRoom)
    {
        PhotonNetwork.JoinOrCreateRoom(
              roomName,
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
        EventData.OnJoinedRoom joinRoom = new EventData.OnJoinedRoom();
        EventManager.Trigger<EventData.OnJoinedRoom>( joinRoom );
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
