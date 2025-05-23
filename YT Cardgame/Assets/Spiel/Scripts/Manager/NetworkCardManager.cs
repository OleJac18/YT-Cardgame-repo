using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkCardManager : NetworkBehaviour
{
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
    }

    private void HandleCardDeckClicked()
    {
        DrawAndSpawnTopCardServerRpc(NetworkManager.LocalClientId);
    }

    private void ServFirstCards(List<ulong> clientIds, ulong currentPlayerId)
    {
        DistributeCardsToPlayers(clientIds);

        int drawnCard = _cardManager.DrawTopCard();
        Debug.Log("Ich habe die Karte " + drawnCard + " f�r das Graveyard gezogen.");
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

            SpawnCardsClientRpc(playerCards, RpcTarget.Single(clientId, RpcTargetUse.Temp));
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
        _cardManager.SetEnemyCardClicked(isSelected, index);
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
}
