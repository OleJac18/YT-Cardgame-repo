using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerUIManager
{
    public static event Action<PlayerNr, Player, bool> InitializePlayerUIEvent;

    public void InitializePlayerUI(Player[] players, ulong currentPlayerId)
    {
        int startIndex = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].id == NetworkManager.Singleton.LocalClientId)
            {
                startIndex = i;
                break;
            }
        }

        for (int i = 0; i < players.Length; i++)
        {
            if (Enum.IsDefined(typeof(PlayerNr), i))
            {
                PlayerNr currentPlayerNr = (PlayerNr)i;

                int playerIndex = (i + startIndex) % players.Length;

                bool isCurrentPlayer = currentPlayerId == players[playerIndex].id;

                InitializePlayerUIEvent?.Invoke(currentPlayerNr, players[playerIndex], isCurrentPlayer);
            }
            else
            {
                Debug.Log("Ungültiger PlayerNr-Wert " + i);
            }
        }
    }
}
