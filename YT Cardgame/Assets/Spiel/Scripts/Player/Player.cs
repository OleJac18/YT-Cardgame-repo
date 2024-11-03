using System.Collections.Generic;

[System.Serializable]
public class Player
{
    public ulong id;
    public List<int> cards;
    public string name;
    public int score;

    public Player(ulong id, List<int> cards, string name, int score)
    {
        this.id = id;
        this.cards = cards;
        this.name = name;
        this.score = score;
    }
}
