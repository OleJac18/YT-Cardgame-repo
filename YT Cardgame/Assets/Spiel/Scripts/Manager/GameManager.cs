using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static event Action<List<ulong>, ulong> ServFirstCardsEvent;

    private PlayerManager _playerManager;
    private TurnManager _turnManager;
    private NetworkPlayerUIManager _networkPlayerUIManager;

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

        return clientIds.Count < 2 ? false : true;
    }

    private void InitializeGame()
    {
        _networkPlayerUIManager = FindObjectOfType<NetworkPlayerUIManager>();

        _turnManager.SetStartPlayer(_playerManager);

        currentPlayerId.Value = _turnManager.GetCurrentPlayerId();

        ServFirstCardsEvent?.Invoke(_playerManager.GetConnectedClientIds(), currentPlayerId.Value);

        _networkPlayerUIManager.InitializePlayerUI(currentPlayerId.Value, _playerManager);
    }

    public void PrintPlayerDictionary()
    {
        _playerManager.PrintPlayerDictionary();
    }
}
