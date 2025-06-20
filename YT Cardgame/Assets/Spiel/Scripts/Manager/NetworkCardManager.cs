using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkCardManager : NetworkBehaviour
{
    public static event Action HidePlayerButtonEvent;

    private CardManager _cardManager;

    // Start is called before the first frame update
    void Start()
    {
        _cardManager = FindObjectOfType<CardManager>();

        GameManager.ServFirstCardsEvent += ServFirstCards;
        CardDeckUI.OnCardDeckClicked += HandleCardDeckClicked;
        CardController.OnCardHoveredEvent += SetEnemyCardHoverEffectClientRpc;
        CardController.OnCardClickedEvent += SetEnemyCardClickedClientRpc;
        CardController.OnGraveyardCardClickedEvent += MoveGraveyardCardToEnemyDrawnPosClientRpc;
        ButtonController.DiscardCardEvent += MoveEnemyCardToGraveyardPos;
        ButtonController.ExchangeCardEvent += ExchangeButtonClicked;
        GameManager.ProcessSelectedCardsEvent += ProcessSelectedCards;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.ServFirstCardsEvent -= ServFirstCards;
        CardDeckUI.OnCardDeckClicked -= HandleCardDeckClicked;
        CardController.OnCardHoveredEvent -= SetEnemyCardHoverEffectClientRpc;
        CardController.OnCardClickedEvent -= SetEnemyCardClickedClientRpc;
        CardController.OnGraveyardCardClickedEvent -= MoveGraveyardCardToEnemyDrawnPosClientRpc;
        ButtonController.DiscardCardEvent -= MoveEnemyCardToGraveyardPos;
        ButtonController.ExchangeCardEvent -= ExchangeButtonClicked;
        GameManager.ProcessSelectedCardsEvent -= ProcessSelectedCards;
    }

    private void HandleCardDeckClicked()
    {
        DrawAndSpawnTopCardServerRpc(NetworkManager.LocalClientId);
    }

    private void ServFirstCards(List<ulong> clientIds, ulong currentPlayerId)
    {
        DistributeCardsToPlayers(clientIds);

        int drawnCard = _cardManager.DrawTopCard();
        Debug.Log("Ich habe die Karte " + drawnCard + " für das Graveyard gezogen.");
        SpawnGraveyardCardClientAndHostRpc(drawnCard, currentPlayerId);
    }

    private void DistributeCardsToPlayers(List<ulong> clientIds)
    {
        foreach (var clientId in clientIds)
        {
            int[] playerCards = new int[4];

            for (int i = 0; i < 4; i++)
            {
                int drawnCard = _cardManager.DrawTopCard();

                if (drawnCard != 100)
                {
                    playerCards[i] = drawnCard;
                }
                else
                {
                    Debug.Log("Kartenstapel ist leer.");
                }
            }

            UpdatePlayerCardsServerRpc(clientId, playerCards);
            SpawnCardsClientRpc(playerCards, RpcTarget.Single(clientId, RpcTargetUse.Temp));
        }
    }

    public void ExchangeButtonClicked()
    {
        if (_cardManager.IsAnyCardSelected())
        {
            HidePlayerButtonEvent?.Invoke();
            HandleCardExchangeClickedServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.Log("Es wurde keine Karte zum Tauschen angeklickt!");
        }
    }

    private void ProcessSelectedCards(int[] cards)
    {
        if (_cardManager.AreSelectedCardsEqual(cards))
        {
            int[] newPlayerCards = _cardManager.UpdatePlayerCards(cards);
            UpdatePlayerCardsServerRpc(NetworkManager.Singleton.LocalClientId, newPlayerCards);

            _cardManager.ExchangePlayerCards(cards);
            ExchangeEnemyCardsClientRpc(cards);
        } 
        else
        {
            // Legt die gezogene Karte auf den Ablagestapel ab
            _cardManager.MovePlayerDrawnCardToGraveyardPos();
            MoveEnemyCardToGraveyardPos();
        }
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void SpawnCardsClientRpc(int[] playerCards, RpcParams rpcParams = default)
    {
        Debug.Log("Ich bin in der SpawnCardsClientRpc Function");
        _cardManager.ServFirstCards(playerCards);
    }

    [Rpc(SendTo.Server)]
    private void DrawAndSpawnTopCardServerRpc(ulong clientId)
    {
        _cardManager.topCardNumber = _cardManager.DrawTopCard();

        if (_cardManager.topCardNumber != 100)
        {
            // Spawned die oberste Karte vom Kartenstapel
            SpawnCardDeckCardSpecificClientRpc(_cardManager.topCardNumber, RpcTarget.Single(clientId, RpcTargetUse.Temp));
        }
        else
        {
            Debug.Log("Kartenstapel ist leer.");
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SpawnCardDeckCardSpecificClientRpc(int cardNumber, RpcParams rpcParams = default)
    {
        _cardManager.SpawnAndMoveCardToPlayerDrawnCardPos(cardNumber, true);

        SpawnCardDeckCardClientRpc();
    }

    [Rpc(SendTo.NotMe)]
    private void SpawnCardDeckCardClientRpc()
    {
        _cardManager.SpawnAndMoveCardToEnemyDrawnCardPos(99, false);
    }

    [Rpc(SendTo.NotMe)]
    private void SetEnemyCardHoverEffectClientRpc(Vector3 scaleBy, int index)
    {
        if (IsServer && !IsHost) return;
        _cardManager.SetEnemyCardHoverEffect(scaleBy, index);
    }

    [Rpc(SendTo.NotMe)]
    private void SetEnemyCardClickedClientRpc(bool isSelected, int index)
    {
        if (IsServer && !IsHost) return;
        _cardManager.SetEnemyCardOutline(isSelected, index);
        _cardManager.SetEnemyClickedCardIndex(isSelected, index);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnGraveyardCardClientAndHostRpc(int cardNumber, ulong currentPlayerId)
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        bool isSelectable = currentPlayerId == localClientId;

        _cardManager.SpawnAndMoveGraveyardCard(cardNumber, isSelectable);
    }

    [Rpc(SendTo.NotMe)]
    private void MoveGraveyardCardToEnemyDrawnPosClientRpc()
    {
        _cardManager.MoveGraveyardCardToEnemyPos();
    }

    private void MoveEnemyCardToGraveyardPos()
    {
        int drawnCardNumber = _cardManager.GetDrawnCardNumber();
        MoveEnemyCardToGraveyardPosClientRpc(drawnCardNumber);
    }


    [Rpc(SendTo.NotMe)]
    private void MoveEnemyCardToGraveyardPosClientRpc(int cardNumber)
    {
        _cardManager.MoveEnemyDrawnCardToGraveyardPos(cardNumber);
    }

    [Rpc(SendTo.NotMe)]
    private void ExchangeEnemyCardsClientRpc(int[] cards)
    {
        _cardManager.ExchangeEnemyCards(cards);
    }

    [Rpc(SendTo.Server)]
    private void UpdatePlayerCardsServerRpc(ulong clientId, int[] cards)
    {
        GameManager.Instance.SetPlayerCards(clientId, cards.ToList<int>());
    }

    [Rpc(SendTo.Server)]
    private void HandleCardExchangeClickedServerRpc(ulong clientId)
    {
        GameManager.Instance.GetPlayerCardsAndProcessSelectedCards(clientId);
    }
}
