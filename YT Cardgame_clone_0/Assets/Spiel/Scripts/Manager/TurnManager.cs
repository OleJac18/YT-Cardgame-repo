using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    private ulong _currentPlayerId;
    private List<ulong> _playerOrder;
    private ulong? _gameEndingPlayerId;

    public void SetStartPlayer(PlayerManager playerManager)
    {
        _playerOrder = playerManager.GetConnectedClientIds();

        _currentPlayerId = _playerOrder[0];
        _gameEndingPlayerId = null;

        Debug.Log("Start Spieler: " + _currentPlayerId);
    }

    public ulong GetCurrentPlayerId()
    {
        return _currentPlayerId;
    }

    public void NextTurn()
    {
        if (_playerOrder.Count == 0)
        {
            Debug.Log("Spielerreihenfolge ist leer");
            return;
        }

        int currentIndex = _playerOrder.IndexOf(_currentPlayerId);
        int nextIndex = (currentIndex + 1) % _playerOrder.Count;
        _currentPlayerId = _playerOrder[nextIndex];

        if(_gameEndingPlayerId == _currentPlayerId)
        {
            Debug.Log("!!! DAS SPIEL IST BEENDET !!!");
        }

        Debug.Log("Nächster Spieler: " + _currentPlayerId);
    }

    public void OnEndGameButtonClicked(ulong clientId)
    {
        if(_gameEndingPlayerId == null)
        {
            _gameEndingPlayerId = clientId;
        }
        
    }
}
