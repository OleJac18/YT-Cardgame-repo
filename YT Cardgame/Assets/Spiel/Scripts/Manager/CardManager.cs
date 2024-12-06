using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _spawnCardDeckPos;
    [SerializeField] private GameObject _spawnCardPlayerPos;
    [SerializeField] private GameObject _spawnCardEnemyPos;
    [SerializeField] private GameObject _showDrawnCardPos;
    [SerializeField] private GameObject _graveyardPos;
    [SerializeField] private GameObject _playerDrawnCardPos;

    public int topCardNumber = -1;

    [SerializeField] private CardStack _cardStack;

    private GameObject _cardDeckCard;
    private GameObject _graveyardCard;

    // Start is called before the first frame update
    void Start()
    {

        if (NetworkManager.Singleton.IsServer)
        {
            _cardStack = new CardStack();
            _cardStack.CreateDeck();
            _cardStack.ShuffleCards();
        }

        CardController.OnGraveyardCardClickedEvent += MoveGraveyardCardToPlayerPos;
    }

    private void OnDestroy()
    {
        CardController.OnGraveyardCardClickedEvent -= MoveGraveyardCardToPlayerPos;
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
            FlipAndMoveCard(_cardDeckCard, target);
        }
        else
        {
            MoveToDrawnPosition(_cardDeckCard, target);
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

    public void SetEnemyCardHoverEffect(Vector3 scaleBy, int index)
    {
        GameObject card = _spawnCardEnemyPos.transform.GetChild(index).gameObject;
        card.transform.localScale = scaleBy;
    }

    public void SetEnemyCardClicked(bool isSelected, int index)
    {
        GameObject card = _spawnCardEnemyPos.transform.GetChild(index).gameObject;
        Outline outline = card.GetComponent<Outline>();

        if (outline == null)
        {
            Debug.Log("Das Objekt hat keine Outline");
        }
        else
        {
            outline.enabled = isSelected;
        }
    }

    public void SpawnAndMoveGraveyardCard(int cardNumber)
    {
        _graveyardCard = SpawnCard(cardNumber, _spawnCardDeckPos, _graveyardPos.transform.parent, Card.Stack.GRAVEYARD, true, false, true);

        CardController controller = _graveyardCard.GetComponent<CardController>();

        Vector3 target = GetCenteredPosition(_graveyardPos.transform);

        LeanTween.move(_graveyardCard, target, 0.5f).setOnComplete(() =>
        {
            _graveyardCard.transform.SetParent(_graveyardPos.transform);
            controller.FlipCardAnimation(false);
        });
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

    private void FlipAndMoveCard(GameObject objectToMove, Transform target)
    {
        CardController controller = objectToMove.GetComponent<CardController>();

        LeanTween.move(objectToMove, _showDrawnCardPos.transform, 0.5f);
        LeanTween.scale(objectToMove, Vector3.one * 1.2f, 0.5f);

        LeanTween.rotateX(objectToMove, 90.0f, 0.25f).setOnComplete(() =>
        {
            controller.SetCardBackImageVisibility(false);
            LeanTween.rotateX(objectToMove, 0.0f, 0.25f).setOnComplete(() =>
            {
                LeanTween.delayedCall(0.5f, () =>
                {
                    MoveToDrawnPosition(objectToMove, target);
                });
            });
        });
    }

    private void MoveToDrawnPosition(GameObject objectToMove, Transform target)
    {
        Vector3 targetPos = GetCenteredPosition(target);

        LeanTween.scale(objectToMove, Vector3.one, 0.5f);

        LeanTween.move(objectToMove, targetPos, 0.5f).setOnComplete(() =>
        {
            objectToMove.transform.SetParent(target);
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

    private void MoveGraveyardCardToPlayerPos()
    {
        MoveGraveyardCardToDrawnPos(_playerDrawnCardPos.transform);
    }

    public void MoveGraveyardCardToDrawnPos(Transform target)
    {
        MoveToDrawnPosition(_graveyardCard, target);

        CardController controller = _graveyardCard.GetComponent<CardController>();
        controller.isSelectable = false;
    }
}
