using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkCardManager : NetworkBehaviour
{
    private PlayerManager _playerManager;
    private CardManager _cardManager;

    // Start is called before the first frame update
    void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _cardManager = FindObjectOfType<CardManager>();

        ConnectionManager.ClientConnectedEvent += CheckAllClientsConnected;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ConnectionManager.ClientConnectedEvent -= CheckAllClientsConnected;
    }

    private void CheckAllClientsConnected(ulong clientId)
    {
        if (IsServer)
        {
            List<ulong> clientIds = _playerManager.GetConnectedClientIds();
            if (clientIds.Count < 2) return;

            DistributeCardsToPlayers(clientIds);
        }
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
}
