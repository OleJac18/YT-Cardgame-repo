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
        Dictionary<ulong, Player> _playerDataDict = playerManager.GetPlayerDataDict();

        foreach(KeyValuePair<ulong, Player> playerData in _playerDataDict)
        {
            Player player = playerData.Value;

            InitalizePlayerUIManagerClientRpc(player.name, player.score, currentPlayerId, RpcTarget.Single(player.id, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void InitalizePlayerUIManagerClientRpc(string playerName, int playerScore, ulong currentPlayerId, RpcParams rpcParams = default)
    {
        _playerUIManager.InitializePlayerUI(playerName, playerScore, currentPlayerId);
    }
}
