using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public enum Stack
    {
        NONE, PLAYERCARD, ENEMYCARD, CARDDECK, GRAVEYARD
    }

    public int number;
    public Stack correspondingDeck;

    public Card()
    {

    }

    public Card(int number, Stack correspondingDeck)
    {
        this.number = number;
        this.correspondingDeck = correspondingDeck;
    }
}
