using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private CardStack _cardStack;

    // Start is called before the first frame update
    void Start()
    {
        _cardStack = new CardStack();
        _cardStack.CreateDeck();
        _cardStack.ShuffleCards();
    }

}
