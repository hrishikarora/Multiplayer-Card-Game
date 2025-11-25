using System;
using UnityEngine;

[Serializable]
public class AbilityData
{
    public string type;
    public int value;
}

[Serializable]
public class CardData
{
    public int id;
    public string name;
    public int cost;
    public int power;
    public AbilityData ability;
    public Color bgColor;
}

[Serializable]
public class CardDataList
{
    public CardData[] cards;
}