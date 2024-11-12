using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<ulong, Player> _playerDataDict = new Dictionary<ulong, Player>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.ClientConnectedEvent += AddNewPlayer;
    }

    private void OnDestroy()
    {
        ConnectionManager.ClientConnectedEvent -= AddNewPlayer;
    }


    private void AddNewPlayer(ulong clientId)
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
}
