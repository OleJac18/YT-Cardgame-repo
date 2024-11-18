using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _spawnCardDeckPos;
    [SerializeField] private GameObject _spawnCardPlayerPos;
    [SerializeField] private GameObject _spawnCardEnemyPos;
    [SerializeField] private GameObject _showDrawnCardPos;

    public int topCardNumber = -1;

    [SerializeField] private CardStack _cardStack;

    private GameObject _cardDeckCard;

    // Start is called before the first frame update
    void Start()
    {

        if (NetworkManager.Singleton.IsServer)
        {
            _cardStack = new CardStack();
            _cardStack.CreateDeck();
            _cardStack.ShuffleCards();
        }
    }

    public int DrawTopCard()
    {
        return _cardStack.DrawTopCard();
    }

    public void SpawnAndMoveCardToDrawnCardPos(int cardNumber, Transform target, bool flipAtDestination)
    {
        // Spawned die oberste Karte vom Kartenstapel
        _cardDeckCard = SpawnCard(cardNumber, _spawnCardDeckPos, _spawnCardDeckPos.transform.parent, Card.Stack.CARDDECK, true, false, false);

        if (flipAtDestination)
        {
            FlipAndMoveCard(target);
        }
        else
        {
            MoveToDrawnPosition(target);
        }


    }

    public void ServFirstCards(int[] playerCards)
    {
        for (int i = 0; i < 4; i++)
        {
            // Spawned die Spielerkarten
            SpawnCard(playerCards[i], _spawnCardPlayerPos, _spawnCardPlayerPos.transform, Card.Stack.PLAYERCARD, false, true, true);

            // Spawned die Gegnerkarten
            SpawnCard(99, _spawnCardEnemyPos, _spawnCardEnemyPos.transform, Card.Stack.ENEMYCARD, true, false, false);
        }
    }

    private GameObject SpawnCard(int cardNumber, GameObject targetPos, Transform parent, Card.Stack corresDeck, bool backCardIsVisible, bool canHover, bool isSelectable)
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

        return spawnCard;
    }

    private void FlipAndMoveCard(Transform target)
    {
        CardController controller = _cardDeckCard.GetComponent<CardController>();

        LeanTween.move(_cardDeckCard, _showDrawnCardPos.transform, 0.5f);
        LeanTween.scale(_cardDeckCard, Vector3.one * 1.2f, 0.5f);

        LeanTween.rotateX(_cardDeckCard, 90.0f, 0.25f).setOnComplete(() =>
        {
            controller.SetCardBackImageVisibility(false);
            LeanTween.rotateX(_cardDeckCard, 0.0f, 0.25f).setOnComplete(() =>
            {
                LeanTween.delayedCall(0.5f, () =>
                {
                    MoveToDrawnPosition(target);
                });
            });
        });
    }

    private void MoveToDrawnPosition(Transform target)
    {
        Vector3 targetPos = GetCenteredPosition(target);

        LeanTween.scale(_cardDeckCard, Vector3.one, 0.5f);

        LeanTween.move(_cardDeckCard, targetPos, 0.5f).setOnComplete(() =>
        {
            _cardDeckCard.transform.SetParent(target);
        });
    }

    private Vector3 GetCenteredPosition(Transform target)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        Vector3 centerOffset = new Vector3(width * (0.5f - rectTransform.pivot.x), height * (0.5f - rectTransform.pivot.y), 0);

        return rectTransform.TransformPoint(centerOffset);
    }

}
