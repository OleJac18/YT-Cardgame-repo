using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _spawnCardDeckPos;
    [SerializeField] private GameObject _spawnCardPlayerPos;
    [SerializeField] private GameObject _spawnCardEnemyPos;


    [SerializeField] private CardStack _cardStack;

    // Start is called before the first frame update
    void Start()
    {
        _cardStack = new CardStack();
        _cardStack.CreateDeck();
        _cardStack.ShuffleCards();

        CardDeckUI.OnCardDeckClicked += HandleCardDeckClicked;
    }

    private void OnDestroy()
    {
        CardDeckUI.OnCardDeckClicked -= HandleCardDeckClicked;
    }

    private void HandleCardDeckClicked()
    {
        int drawnCard = _cardStack.DrawTopCard();

        if (drawnCard != 100)
        {
            // Spawned die oberste Karte vom Kartenstapel
            SpawnCard(drawnCard, _spawnCardDeckPos, _spawnCardDeckPos.transform.parent, Card.Stack.CARDDECK, true, false, true);
        }
        else
        {
            Debug.Log("Kartenstapel ist leer.");
        }
    }

    public void ServFirstCards()
    {
        for (int i = 0; i < 4; i++)
        {
            int drawnCard = _cardStack.DrawTopCard();

            if (drawnCard != 100)
            {
                // Spawned die Spielerkarten
                SpawnCard(drawnCard, _spawnCardPlayerPos, _spawnCardPlayerPos.transform, Card.Stack.PLAYERCARD, false, true, true);

                // Spawned die Gegnerkarten
                SpawnCard(99, _spawnCardEnemyPos, _spawnCardEnemyPos.transform, Card.Stack.ENEMYCARD, true, false, false);
            }
            else
            {
                Debug.Log("Kartenstapel ist leer.");
            }
        }
    }

    private void SpawnCard(int cardNumber, GameObject targetPos, Transform parent, Card.Stack corresDeck, bool backCardIsVisible, bool canHover, bool isSelectable)
    {
        GameObject spawnCard = Instantiate(_cardPrefab, targetPos.transform.position, targetPos.transform.rotation);

        spawnCard.transform.SetParent(parent);
        spawnCard.transform.localScale = Vector3.one;

        CardController controller = spawnCard.GetComponent<CardController>();
        controller.SetCorrespondingDeck(corresDeck);
        controller.SetCardBackImageVisibility(backCardIsVisible);
        controller.CardNumber = cardNumber;
        controller.canHover = canHover;
        controller.isSelectable = isSelectable;
    }

}
