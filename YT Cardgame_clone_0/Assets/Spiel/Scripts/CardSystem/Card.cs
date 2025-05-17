
[System.Serializable]
public class Card
{
    public enum DeckType
    {
        NONE, PLAYERCARD, ENEMYCARD, CARDDECK, GRAVEYARD, DRAWNCARD
    }

    public int number;
    public DeckType correspondingDeck;

    public Card()
    {

    }

    public Card(int number, DeckType correspondingDeck)
    {
        this.number = number;
        this.correspondingDeck = correspondingDeck;
    }
}
