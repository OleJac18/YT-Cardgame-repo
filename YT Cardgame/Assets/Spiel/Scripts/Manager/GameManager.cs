using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<List<ulong>, ulong> ServFirstCardsEvent;
    public static event Action<ulong> SetStartSettingsEvent;

    private PlayerManager _playerManager;
    private TurnManager _turnManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        _playerManager = new PlayerManager();
        _turnManager = new TurnManager();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            _turnManager.NextTurn();
        }
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

        _turnManager.SetStartPlayer(_playerManager);

        ulong currentPlayerId = _turnManager.GetCurrentPlayerId();

        SetStartSettingsEvent?.Invoke(currentPlayerId);

        ServFirstCardsEvent?.Invoke(clientIds, currentPlayerId);
    }

    public void PrintPlayerDictionary()
    {
        _playerManager.PrintPlayerDictionary();
    }
}
