using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Button discardButton;
    public Button exchangeButton;
    public Button endGameButton;

    public static event Action DiscardCardEvent;
    public static event Action ExchangeCardEvent;
    public static event Action<ulong> EndGameButtonClickedEvent;

    // Start is called before the first frame update
    void Start()
    {
        CardManager.ShowPlayerButtonEvent += ShowPlayerButton;
        NetworkCardManager.HidePlayerButtonEvent += HidePlayerButton;
        GameManager.Instance.currentPlayerId.OnValueChanged += ShowEndGameButton;
        CardDeckUI.OnCardDeckClicked += HideEndGameButton;
        CardController.OnGraveyardCardClickedEvent += HideEndGameButton;

        HidePlayerButton();
    }

    void OnDestroy()
    {
        CardManager.ShowPlayerButtonEvent -= ShowPlayerButton;
        NetworkCardManager.HidePlayerButtonEvent -= HidePlayerButton;
        GameManager.Instance.currentPlayerId.OnValueChanged -= ShowEndGameButton;
        CardDeckUI.OnCardDeckClicked -= HideEndGameButton;
        CardController.OnGraveyardCardClickedEvent -= HideEndGameButton;
    }

    private void HidePlayerButton()
    {
        discardButton.gameObject.SetActive(false);
        exchangeButton.gameObject.SetActive(false);
        endGameButton.gameObject.SetActive(false);
    }

    private void HideEndGameButton()
    {
        endGameButton.gameObject.SetActive(false);
    }

    private void ShowPlayerButton()
    {
        if (NetworkManager.Singleton.LocalClientId != GameManager.Instance.currentPlayerId.Value) return;

        discardButton.gameObject.SetActive(true);
        exchangeButton.gameObject.SetActive(true);
    }

    private void ShowEndGameButton(ulong previousValue, ulong newValue)
    {
        if (NetworkManager.Singleton.LocalClientId != newValue) return;
        endGameButton.gameObject.SetActive(true);
    }

    public void DiscardButtonClicked()
    {
        Debug.Log("Ich möchte die Karte wieder abgeben");
        HidePlayerButton();

        DiscardCardEvent?.Invoke();
    }

    public void ExchangeButtonClicked()
    {
        Debug.Log("Ich möchte eine Karte tauschen");

        ExchangeCardEvent?.Invoke();
    }

    public void EndGameButtonClicked()
    {
        Debug.Log("Ich möchte das Spiel beenden.");
        HideEndGameButton();

        EndGameButtonClickedEvent?.Invoke(NetworkManager.Singleton.LocalClientId);
    }
}
