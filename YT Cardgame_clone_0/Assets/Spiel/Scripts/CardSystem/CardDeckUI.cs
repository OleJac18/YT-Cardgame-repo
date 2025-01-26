using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDeckUI : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnCardDeckClicked;
    private bool isSelectable;

    public void Start()
    {
        GameManager.SetStartSettingsEvent += SetIsSelectableState;
    }

    public void OnDestroy()
    {
        GameManager.SetStartSettingsEvent -= SetIsSelectableState;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelectable) return;

        OnCardDeckClicked?.Invoke();
        Debug.Log("CardDeck geklickt.");
    }

    private void SetIsSelectableState(ulong currentPlayerId)
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        isSelectable = currentPlayerId == localClientId;
    }
}
