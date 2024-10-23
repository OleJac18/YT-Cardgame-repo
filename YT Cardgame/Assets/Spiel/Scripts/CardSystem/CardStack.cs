using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardStack
{
    public List<Card> stack = new List<Card>();

    public void CreateDeck()
    {
        for (int i = 0; i < 52; i++)
        {
            // Für die 0
            if (i < 2)
            {
                stack.Add(new Card(0, Card.Stack.CARDDECK));
            }

            // Für 1 bis 12
            if (i >= 2 && i < 50)
            {
                int number = ((i - 2) / 4) + 1;

                stack.Add(new Card(number, Card.Stack.CARDDECK));
            }

            // Für die 13
            if (i >= 50)
            {
                stack.Add(new Card(13, Card.Stack.CARDDECK));
            }
        }
    }

    public void ShuffleCards()
    {
        for (int i = 0; i < stack.Count; i++)
        {
            int r = Random.Range(0, stack.Count - 1);
            Card tmp = stack[i];
            stack[i] = stack[r];
            stack[r] = tmp;
        }
    }

    public int DrawTopCard()
    {
        if (stack.Count == 0)
        {
            Debug.Log("Kann keine Karte von einem leerem Stapel ziehen.");
            return 100;
        }

        int topCardIndex = stack.Count - 1;
        int number = stack[topCardIndex].number;
        stack.RemoveAt(topCardIndex);
        return number;
    }
}
