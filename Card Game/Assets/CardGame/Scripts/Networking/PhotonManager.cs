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


        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = GameConstants.MAX_PLAYERS,
            PlayerTtl = 21000,
            CleanupCacheOnLeave = false
        };


        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { GameConstants.P1, "" },
            { GameConstants.P2, "" }
        };

        roomOptions.CustomRoomPropertiesForLobby = new string[]
        {
             GameConstants.P1,
             GameConstants.P2
        };

        PhotonNetwork.JoinOrCreateRoom(
              _roomName,
             roomOptions,
              TypedLobby.Default
          );

    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.PlayerCount}/2");
        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        if (props.ContainsKey(GameConstants.P1) &&
                (string)props[GameConstants.P1] == PhotonNetwork.LocalPlayer.UserId)
        {
            PhotonNetwork.NickName = GameConstants.P1;
            Debug.Log("Rejoined as Player 1");
            return;
        }

        if (props.ContainsKey(GameConstants.P2) &&
        (string)props[GameConstants.P2] == PhotonNetwork.LocalPlayer.UserId)
        {
            PhotonNetwork.NickName = GameConstants.P2;
            Debug.Log("Rejoined as Player 2");
            return;
        }

        if (PhotonNetwork.IsMasterClient && (string)props[GameConstants.P1] == "")
        {
            Debug.Log("Assigning Player 1");
            SetRoomProp(GameConstants.P1, PhotonNetwork.LocalPlayer.UserId);
            PhotonNetwork.NickName = GameConstants.P1;
        }
        else
        {
            Debug.Log("Assigning Player 2");
            SetRoomProp(GameConstants.P2, PhotonNetwork.LocalPlayer.UserId);
            PhotonNetwork.NickName = GameConstants.P2;
        }
    }

    private void SetRoomProp(string key, object value)
    {
        var hash = new ExitGames.Client.Photon.Hashtable
    {
        { key, value }
    };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join room failed: {returnCode} - {message}");

        EventActionData.OnJoinedRoom joinRoom = new EventActionData.OnJoinedRoom();
        EventManager.Trigger<EventActionData.OnJoinedRoom>(joinRoom);   
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

    private string GenerateRoomName()
    {
        return "Room_" + Random.Range(100000, 999999);
    }

    private byte maxPlayers => GameConstants.MAX_PLAYERS;

}
