using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerNr
{
    Player1 = 0,
    Player2 = 1,
    Player3 = 2,
    Player4 = 3
}


public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private Image activePlayerImage;

    [SerializeField] PlayerNr _playerNr;

    private ulong _localPlayerId;

    private void Start()
    {
        PlayerUIManager.InitializePlayerUIEvent += Initialize;
        GameManager.Instance.currentPlayerId.OnValueChanged += OnPlayerTurnChanged;
    }

    private void OnDestroy()
    {
        PlayerUIManager.InitializePlayerUIEvent -= Initialize;
        GameManager.Instance.currentPlayerId.OnValueChanged -= OnPlayerTurnChanged;
    }

    private void Initialize(PlayerNr playerNr, Player player, bool isCurrentPlayer)
    {
        if(_playerNr == playerNr)
        {
            _localPlayerId = player.id;
            playerNameText.text = player.name;
            UpdateScore(player.score);
            SetActivePlayer(isCurrentPlayer);
        }
    }

    public void UpdateScore(int score)
    {
        playerScoreText.text = "Score: " + score;
    }

    public void SetActivePlayer(bool isCurrentPlayer)
    {
        activePlayerImage.color = isCurrentPlayer ? Color.green : Color.grey;
    }

    private void OnPlayerTurnChanged(ulong previousPlayerId, ulong currentPlayerId)
    {
        bool isCurrentPlayer = currentPlayerId == _localPlayerId;

        SetActivePlayer(isCurrentPlayer);
    }
}
