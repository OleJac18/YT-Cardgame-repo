using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static event Action<List<ulong>, ulong> ServFirstCardsEvent;

    private PlayerManager _playerManager;
    private TurnManager _turnManager;

    public NetworkVariable<ulong> currentPlayerId = new NetworkVariable<ulong>();
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;

        _playerManager = new PlayerManager();
        _turnManager = new TurnManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.ClientConnectedEvent += OnClientConnected;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ConnectionManager.ClientConnectedEvent -= OnClientConnected;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        base.OnNetworkSpawn();

        currentPlayerId.Value = 50;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            _turnManager.NextTurn();
            currentPlayerId.Value = _turnManager.GetCurrentPlayerId();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        _playerManager.AddNewPlayer(clientId);

        if (CheckAllClientsConnected())
        {
            InitializeGame();
        }
    }

    private bool CheckAllClientsConnected()
    {
        List<ulong> clientIds = _playerManager.GetConnectedClientIds();
        if (clientIds.Count < 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void InitializeGame()
    {
        _turnManager.SetStartPlayer(_playerManager);

        currentPlayerId.Value = _turnManager.GetCurrentPlayerId();

        ServFirstCardsEvent?.Invoke(_playerManager.GetConnectedClientIds(), currentPlayerId.Value);
    }

    public void PrintPlayerDictionary()
    {
        _playerManager.PrintPlayerDictionary();
    }
}
