using System;
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
    [SerializeField] private GameObject _enemyDrawnCardPos;

    public int topCardNumber = -1;

    [SerializeField] private CardStack _cardStack;

    [SerializeField] private GameObject _cardDeckCard;
    [SerializeField] private GameObject _graveyardCard;
    [SerializeField] private GameObject _drawnCard;

    [SerializeField] private bool[] _playerClickedCards;
    [SerializeField] private bool[] _enemyClickedCards;

    public static event Action ShowPlayerButtonEvent;
    public static event Action EndTurnEvent;

    // Start is called before the first frame update
    void Start()
    {
        _playerClickedCards = new bool[4];
        _enemyClickedCards = new bool[4];

        if (NetworkManager.Singleton.IsServer)
        {
            _cardStack = new CardStack();
            _cardStack.CreateDeck();
            _cardStack.ShuffleCards();
        }

        CardController.OnGraveyardCardClickedEvent += MoveGraveyardCardToPlayerPos;
        ButtonController.DiscardCardEvent += MovePlayerDrawnCardToGraveyardPos;
        CardController.OnCardClickedEvent += SetPlayerClickedCardIndex;
    }

    private void OnDestroy()
    {
        CardController.OnGraveyardCardClickedEvent -= MoveGraveyardCardToPlayerPos;
        ButtonController.DiscardCardEvent -= MovePlayerDrawnCardToGraveyardPos;
        CardController.OnCardClickedEvent -= SetPlayerClickedCardIndex;
    }

    public int DrawTopCard()
    {
        return _cardStack.DrawTopCard();
    }

    public int GetDrawnCardNumber()
    {
        CardController controller = _drawnCard.GetComponent<CardController>();
        return controller.CardNumber;
    }

    public void SpawnAndMoveCardToPlayerDrawnCardPos(int cardNumber, bool flipAtDestination)
    {
        SpawnAndMoveCardToDrawnCardPos(cardNumber, _playerDrawnCardPos.transform, flipAtDestination);
    }

    public void SpawnAndMoveCardToEnemyDrawnCardPos(int cardNumber, bool flipAtDestination)
    {
        SpawnAndMoveCardToDrawnCardPos(cardNumber, _enemyDrawnCardPos.transform, flipAtDestination);
    }

    private void SpawnAndMoveCardToDrawnCardPos(int cardNumber, Transform target, bool flipAtDestination)
    {
        // Spawned die oberste Karte vom Kartenstapel
        _cardDeckCard = SpawnCard(cardNumber, _spawnCardDeckPos, _spawnCardDeckPos.transform.parent, Card.DeckType.CARDDECK, true, false, false);

        if (flipAtDestination)
        {
            FlipAndMoveCard(_cardDeckCard, target);
        }
        else
        {
            MoveToDrawnPosition(_cardDeckCard, target, false);
        }


    }

    public void ServFirstCards(int[] playerCards)
    {
        for (int i = 0; i < 4; i++)
        {
            // Spawned die Spielerkarten
            SpawnCard(playerCards[i], _spawnCardPlayerPos, _spawnCardPlayerPos.transform, Card.DeckType.PLAYERCARD, false, true, true);

            // Spawned die Gegnerkarten
            SpawnCard(99, _spawnCardEnemyPos, _spawnCardEnemyPos.transform, Card.DeckType.ENEMYCARD, true, false, false);
        }
    }

    public void SetEnemyCardHoverEffect(Vector3 scaleBy, int index)
    {
        GameObject card = _spawnCardEnemyPos.transform.GetChild(index).gameObject;
        card.transform.localScale = scaleBy;
    }

    public void SetEnemyCardOutline(bool isSelected, int index)
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

    public void SpawnAndMoveGraveyardCard(int cardNumber, bool isSelectable)
    {
        _graveyardCard = SpawnCard(cardNumber, _spawnCardDeckPos, _graveyardPos.transform.parent, Card.DeckType.GRAVEYARD, true, false, isSelectable);

        CardController controller = _graveyardCard.GetComponent<CardController>();

        Vector3 target = GetCenteredPosition(_graveyardPos.transform);

        LeanTween.move(_graveyardCard, target, 0.5f).setOnComplete(() =>
        {
            _graveyardCard.transform.SetParent(_graveyardPos.transform);
            controller.FlipCardAnimation(false);
        });
    }

    private GameObject SpawnCard(int cardNumber, GameObject targetPos, Transform parent, Card.DeckType corresDeck, bool backCardIsVisible, bool canHover, bool isSelectable)
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
                    MoveToDrawnPosition(objectToMove, target, true);
                });
            });
        });
    }

    private void MoveToDrawnPosition(GameObject objectToMove, Transform target, bool showButton)
    {
        Vector3 targetPos = GetCenteredPosition(target);

        LeanTween.scale(objectToMove, Vector3.one, 0.5f);

        LeanTween.move(objectToMove, targetPos, 0.5f).setOnComplete(() =>
        {
            objectToMove.transform.SetParent(target);

            SetCardToDrawnCard(objectToMove, showButton);
        });
    }

    private void SetCardToDrawnCard(GameObject objectToMove, bool showButton)
    {
        _drawnCard = objectToMove;

        CardController controller = objectToMove.GetComponent<CardController>();
        Card.DeckType corresDeck = controller.GetCorrespondingDeck();

        if (corresDeck == Card.DeckType.GRAVEYARD)
            _graveyardCard = null;
        else
            _cardDeckCard = null;

        controller.SetCorrespondingDeck(Card.DeckType.DRAWNCARD);

        if (showButton)
            ShowPlayerButtonEvent?.Invoke();
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
        MoveGraveyardCardToDrawnPos(_playerDrawnCardPos.transform, true);
    }

    public void MoveGraveyardCardToEnemyPos()
    {
        MoveGraveyardCardToDrawnPos(_enemyDrawnCardPos.transform, false);
    }

    public void MoveGraveyardCardToDrawnPos(Transform target, bool showButton)
    {
        MoveToDrawnPosition(_graveyardCard, target, showButton);

        CardController controller = _graveyardCard.GetComponent<CardController>();
        controller.isSelectable = false;
    }

    public void MovePlayerDrawnCardToGraveyardPos()
    {
        CardController controller = _drawnCard.GetComponent<CardController>();
        MoveDrawnCardToGraveyardPos(controller.CardNumber);
    }

    public void MoveEnemyDrawnCardToGraveyardPos(int cardNumber)
    {
        MoveDrawnCardToGraveyardPos(cardNumber);
    }

    public void MoveDrawnCardToGraveyardPos(int cardNumber)
    {
        Vector3 targetPos = GetCenteredPosition(_graveyardPos.transform);

        LeanTween.move(_drawnCard, targetPos, 0.5f).setOnComplete(() =>
        {
            _drawnCard.transform.SetParent(_graveyardPos.transform);

            CardController drawnCardController = _drawnCard.GetComponent<CardController>();
            drawnCardController.CardNumber = cardNumber;

            if (drawnCardController.GetCardBackImageVisibility())
            {
                drawnCardController.FlipCardAnimation(false);
            }

            _graveyardCard = _drawnCard;

            CardController graveyardController = _graveyardCard.GetComponent<CardController>();
            graveyardController.SetCorrespondingDeck(Card.DeckType.GRAVEYARD);

            _drawnCard = null;

            EndTurnEvent?.Invoke();
        });
    }

    private void SetPlayerClickedCardIndex(bool isSelected, int index)
    {
        _playerClickedCards[index] = isSelected;
    }

    public void SetEnemyClickedCardIndex(bool isSelected, int index)
    {
        _enemyClickedCards[index] = isSelected;
    }

    private int FindFirstTrueIndex(bool[] _clickedCards)
    {
        for (int i = 0; i < _clickedCards.Length; i++)
        {
            if (_clickedCards[i])
            {
                return i; // Gibt den Index des ersten Elements zurück, das true ist
            }
        }
        return -1; // Gibt -1 zurück, wenn kein true gefunden wurde
    }

    public bool IsAnyCardSelected()
    {
        foreach (bool value in _playerClickedCards)
        {
            if (value)
            {
                return true; // Sofort beenden, wenn ein true gefunden wurde
            }
        }
        return false; // Kein true gefunden
    }

    public int[] UpdatePlayerCards(int[] cards)
    {
        List<int> newCardsList = new List<int>();
        bool addedClickedCard = false; // Verfolgt, ob bereits eine geklickte Karte hinzugefügt wurde

        for (int i = 0; i < cards.Length; i++)
        {
            if (!_playerClickedCards[i])
            {
                // Füge alle nicht geklickten Karten hinzu
                newCardsList.Add(cards[i]);
            }
            else if (!addedClickedCard)
            {
                // Füge die gezogene Karte hinzu, wenn noch keine hinzugefügt wurde
                CardController controller = _drawnCard.GetComponent<CardController>();
                newCardsList.Add(controller.CardNumber);
                addedClickedCard = true;
            }
        }

        return newCardsList.ToArray();
    }

    public bool AreSelectedCardsEqual(int[] cards)
    {
        int? referenceValue = null;

        for (int i = 0; i < cards.Length; i++)
        {
            if (_playerClickedCards[i])
            {
                if (referenceValue == null)
                {
                    referenceValue = cards[i];
                }
                else if (cards[i] != referenceValue)
                {
                    Debug.Log("Die angeklickten Karten sind nicht gleich. Gezogene Karte wird abgelegt.");
                    return false;
                }
            }
        }

        return true;
    }

    private void ResetClickedCards(bool[] clickedCards)
    {
        Array.Fill(clickedCards, false);
    }

    public void ExchangeEnemyCards(int[] cards)
    {
        ExchangeCards(_spawnCardEnemyPos, _enemyClickedCards, cards);
    }

    public void ExchangePlayerCards(int[] cards)
    {
        if (!IsAnyCardSelected()) return; 
        ExchangeCards(_spawnCardPlayerPos, _playerClickedCards, cards);
    }

    private void ExchangeCards(GameObject playerPanel, bool[] clickedCards, int[] cards)
    {
        MovePlayerCardsToGraveyardPos(playerPanel, clickedCards, cards);
        MoveDrawnCardToTarget(playerPanel, clickedCards);
    }

    private void MovePlayerCardsToGraveyardPos(GameObject playerPanel, bool[] clickedCards, int[] cards)
    {
        // Erste Karte finden, die geklickt wurde
        int index = FindFirstTrueIndex(clickedCards);
        int cardNumber = cards[index];

        Vector3 targetPos = GetCenteredPosition(_graveyardPos.transform);

        for (int i = 0; i < clickedCards.Length; i++)
        {
            if (!clickedCards[i]) { continue; }

            GameObject _selectedCard = playerPanel.transform.GetChild(i).gameObject;
            CardController controller = _selectedCard.GetComponent<CardController>();
            controller.SetOutline(false);
            controller.CardNumber = cardNumber;

            LeanTween.move(_selectedCard, targetPos, 0.5f).setOnComplete(() =>
            {
                _graveyardCard = _selectedCard;

                _selectedCard.transform.SetParent(_graveyardPos.transform);
                controller.SetCorrespondingDeck(Card.DeckType.GRAVEYARD);

                controller.FlipCardAnimation(false);
            });
        }
    }

    private void MoveDrawnCardToTarget(GameObject playerPanel, bool[] clickedCards)
    {
        int index = FindFirstTrueIndex(clickedCards);
        GameObject _firstSelectedCard = playerPanel.transform.GetChild(index).gameObject;

        // Herausfinden, ob man der aktuelle Spieler ist, damit man sagen kann, wie der Bogen bei der Bewegung sein soll
        int rotation;
        bool isCurrentPlayer = GameManager.Instance.currentPlayerId.Value == NetworkManager.Singleton.LocalClientId;

        rotation = isCurrentPlayer ? 1 : -1;

        Vector3[] points = MoveInCircle.CalculateCircle(8, _drawnCard.transform,
            _firstSelectedCard.transform, rotation, 100);

        LeanTween.moveSpline(_drawnCard, points, 0.5f).setOnComplete(() =>
        {
            _drawnCard.transform.SetParent(playerPanel.transform);
            _drawnCard.transform.SetSiblingIndex(index);

            CardController controller = _drawnCard.GetComponent<CardController>();
            Card.DeckType corresDeck = isCurrentPlayer ? Card.DeckType.PLAYERCARD : Card.DeckType.ENEMYCARD;

            controller.SetCorrespondingDeck(corresDeck);

            _drawnCard = null;

            ResetClickedCards(_playerClickedCards);
            ResetClickedCards(_enemyClickedCards);

            LeanTween.delayedCall(0.6f, () =>
            {
                EndTurnEvent?.Invoke();
            });
        });
    }
}
