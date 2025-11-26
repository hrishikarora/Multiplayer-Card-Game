using System.Collections.Generic;
using UnityEngine;

public class GameSnapshot
{
    private List<int> p1HandIds = new();
    private List<int> p1PlacedIds = new();
    private List<int> p2HandIds = new();
    private List<int> p2PlacedIds = new();
    private int p1Score;
    private int p2Score;
    private int currentTurn;
}
