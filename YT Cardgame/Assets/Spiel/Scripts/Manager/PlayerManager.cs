using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    private Dictionary<ulong, Player> _playerDataDict = new Dictionary<ulong, Player>();

    public void AddNewPlayer(ulong clientId)
    {
        Debug.Log("Ich will einen neuen Spieler hinzufügen");

        if (!_playerDataDict.ContainsKey(clientId))
        {
            _playerDataDict[clientId] = new Player(clientId, new List<int>(), "Player " + clientId, 0);
            Debug.Log("Player " + clientId + " hat sich verbunden");
        }

    }

    public void PrintPlayerDictionary()
    {
        foreach (KeyValuePair<ulong, Player> playerData in _playerDataDict)
        {
            ulong id = playerData.Key;
            Player player = playerData.Value;

            Debug.Log("ID: " + id + ", Name: " + player.name + ", Level: " + player.score);
        }
    }

    public List<ulong> GetConnectedClientIds()
    {
        return new List<ulong>(_playerDataDict.Keys);
    }

    public Dictionary<ulong, Player> GetPlayerDataDict()
    {
        return _playerDataDict;
    }
}
