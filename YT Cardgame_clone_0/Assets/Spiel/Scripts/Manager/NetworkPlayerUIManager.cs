using System.Collections.Generic;
using Unity.Netcode;

public class NetworkPlayerUIManager : NetworkBehaviour
{
    private readonly PlayerUIManager _playerUIManager;

    public NetworkPlayerUIManager()
    {
        _playerUIManager = new PlayerUIManager();
    }

    public void InitializePlayerUI(ulong currentPlayerId, PlayerManager playerManager)
    {
        Player[] players = playerManager.GetAllPlayers();

        InitalizePlayerUIManagerClientsAndHostRpc(players, currentPlayerId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InitalizePlayerUIManagerClientsAndHostRpc(Player[] players, ulong currentPlayerId, RpcParams rpcParams = default)
    {
        _playerUIManager.InitializePlayerUI(players, currentPlayerId);
    }
}
