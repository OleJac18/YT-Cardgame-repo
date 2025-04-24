using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Button discardButton;
    public Button exchangeButton;

    // Start is called before the first frame update
    void Start()
    {
        CardManager.ShowPlayerButtonEvent += ShowPlayerButton;

        HidePlayerButton();
    }

    void OnDestroy()
    {
        CardManager.ShowPlayerButtonEvent -= ShowPlayerButton;
    }

    private void HidePlayerButton()
    {
        discardButton.gameObject.SetActive(false);
        exchangeButton.gameObject.SetActive(false);
    }

    private void ShowPlayerButton()
    {
        if (NetworkManager.Singleton.LocalClientId != GameManager.Instance.currentPlayerId.Value) return;

        discardButton.gameObject.SetActive(true);
        exchangeButton.gameObject.SetActive(true);
    }

    public void DiscardButtonClicked()
    {
        Debug.Log("Ich möchte die Karte wieder abgeben");
        HidePlayerButton();
    }

    public void ExchangeButtonClicked()
    {
        Debug.Log("Ich möchte die Karte wieder abgeben");
        HidePlayerButton();
    }
}
