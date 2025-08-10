using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static event Action<List<ulong>, ulong> ServFirstCardsEvent;
    public static event Action<int[]> ProcessSelectedCardsEvent;

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
        CardManager.EndTurnEvent += EndTurn;
        ButtonController.EndGameButtonClickedEvent += OnEndGameButtonClickedServerRpc;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ConnectionManager.ClientConnectedEvent -= OnClientConnected;
        CardManager.EndTurnEvent -= EndTurn;
        ButtonController.EndGameButtonClickedEvent -= OnEndGameButtonClickedServerRpc;
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

    private void EndTurn()
    {
        if(IsServer && _turnManager != null)
        {
            _turnManager.NextTurn();
            currentPlayerId.Value = _turnManager.GetCurrentPlayerId();
        }
    }

    public void SetPlayerCards(ulong clientId, List<int> cards)
    {
        _playerManager.SetPlayerCards(clientId, cards);
    }

    public void GetPlayerCardsAndProcessSelectedCards(ulong clientId)
    {
        List<int> cards = _playerManager.GetPlayerCards(clientId);
        ProcessSelectedCardsClienRpc(cards.ToArray(), RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void ProcessSelectedCardsClienRpc(int[] cards, RpcParams rpcParams = default)
    {
        ProcessSelectedCardsEvent?.Invoke(cards);
    }

    [Rpc(SendTo.Server)]
    private void OnEndGameButtonClickedServerRpc(ulong clientId)
    {
        _turnManager.OnEndGameButtonClicked(clientId);
        EndTurn();
    }
}
