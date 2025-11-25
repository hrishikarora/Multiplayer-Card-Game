[System.Serializable]
public class TurnStartMessage
{
    public string action;
    public int turnNumber;
    public int availableCost;
}

[System.Serializable]
public class RevealCardsMessage
{
    public string action;
    public int[] cardIds;
    public string playerId;
}

[System.Serializable]
public class GameEndMessage
{
    public string action;
    public string winner;
    public int p1Score;
    public int p2Score;
}


[System.Serializable]
public class GameStartMessage
{
    public string action;
    public string[] playerIds;
    public int totalTurns;
}

[System.Serializable]
public class EndTurnMessage
{
    public string action;
    public string playerId;
}
[System.Serializable]
public class RequestCardMessage
{
    public string action;
    public string playerId;
}
