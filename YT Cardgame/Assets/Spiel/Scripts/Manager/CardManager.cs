using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _spawnCardDeckPos;


    [SerializeField] private CardStack _cardStack;

    // Start is called before the first frame update
    void Start()
    {
        _cardStack = new CardStack();
        _cardStack.CreateDeck();
        _cardStack.ShuffleCards();

        CardDeckUI.OnCardDeckClicked += HandleCardDeckClicked;
    }

    private void HandleCardDeckClicked()
    {
        int drawnCard = _cardStack.DrawTopCard();

        if (drawnCard != 100)
        {
            SpawnCard(drawnCard);
        }
        else
        {
            Debug.Log("Kartenstapel ist leer.");
        }
    }

    private void SpawnCard(int topCardNumber)
    {
        GameObject spawnedCard = Instantiate(_cardPrefab, _spawnCardDeckPos.transform.position, _spawnCardDeckPos.transform.rotation);

        spawnedCard.transform.SetParent(_spawnCardDeckPos.transform.parent);
        spawnedCard.transform.localScale = Vector3.one;

        CardController controller = spawnedCard.GetComponent<CardController>();
        controller.SetCardBackImageVisibility(true);
        controller.SetCorrespondingDeck(Card.Stack.CARDDECK);
        controller.CardNumber = topCardNumber;

        LeanTween.move(spawnedCard, new Vector3(300, 300, 0), 0.5f);
    }
}
