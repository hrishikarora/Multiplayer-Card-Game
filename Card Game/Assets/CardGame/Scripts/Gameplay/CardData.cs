using System;

[Serializable]
public class Ability
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
    public Ability ability;
}

[Serializable]
public class CardDataList
{
    public CardData[] cards;
}