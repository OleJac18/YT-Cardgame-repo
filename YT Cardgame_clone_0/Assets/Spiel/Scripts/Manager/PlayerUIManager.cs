using System;
using Unity.Netcode;

public class PlayerUIManager
{
    public static event Action<string, int, bool> InitializePlayerUIEvent;

    public void InitializePlayerUI(string playerName, int playerScore, ulong currentPlayerId)
    {
        bool isCurrentPlayer = currentPlayerId == NetworkManager.Singleton.LocalClientId;

        InitializePlayerUIEvent?.Invoke(playerName, playerScore, isCurrentPlayer);
    }
}
