using UnityEngine;

public static class GameConstants
{
    public const string GAME_SCENE = "GameScene";
    public const string LOBBY_SCENE = "LobbyScene";


    #region Gameplay
    public const int MAX_PLAYERS = 2;
    public const string P1 = "P1";
    public const string P2 = "P2";
    public const int TOTAL_TURNS = 6;
    #endregion

    #region Photon Message
    public const byte GAME_EVENT_CODE = 2;
    public const string GAME_START_ACTION_NAME = "gameStart";
    public const string TURN_START_ACTION_NAME = "turnStart";
    #endregion

}