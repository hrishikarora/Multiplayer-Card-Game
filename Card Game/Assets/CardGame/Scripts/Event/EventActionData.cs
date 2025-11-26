using System;
using System.Collections.Generic;

public static class EventActionData
{
 
    public class CreateJoinRoom : GameEvent { }
    public class ConnectedToMaster : GameEvent { }
    public class OnJoinedRoom : GameEvent { }

    public class GameStart : GameEvent
    {
        public string[] playerIds;
        public int totalTurns;
    }

    public class TurnStart : GameEvent
    {
        public int turnNumber;
        public int availableCost;
    }

    public class PlayerEndedTurn : GameEvent
    {
        public string playerId;
    }

    public class RevealCards : GameEvent
    {
        public int[] cardIds;
        public string playerId;
    }

    public class GameEnd : GameEvent
    {
        public string winner;
        public int p1Score;
        public int p2Score;
    }

    public class GetPlayerCards : GameEvent
    {
        public string playerId;
        public List<int> cardIds;
    }

    public class SendRevealCards : GameEvent
    {
        public string PlayerId;
    }

    public class OnPlayerDisconnected : GameEvent
    {

    }


    public class OnPlayerReconnected : GameEvent
    {

    }
}