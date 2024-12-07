using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<List<ulong>> ServFirstCardsEvent;

    private PlayerManager _playerManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        _playerManager = new PlayerManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.ClientConnectedEvent += OnClientConnected;
    }

    private void OnDestroy()
    {
        ConnectionManager.ClientConnectedEvent -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        _playerManager.AddNewPlayer(clientId);

        CheckAllClientsConnected();
    }

    private void CheckAllClientsConnected()
    {
        List<ulong> clientIds = _playerManager.GetConnectedClientIds();
        if (clientIds.Count < 2) return;

        ServFirstCardsEvent?.Invoke(clientIds);
    }

    public void PrintPlayerDictionary()
    {
        _playerManager.PrintPlayerDictionary();
    }
}
