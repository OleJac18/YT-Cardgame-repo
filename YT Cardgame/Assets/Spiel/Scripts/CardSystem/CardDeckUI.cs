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
        GameManager.Instance.currentPlayerId.OnValueChanged += SetSelectableState;
    }

    public void OnDestroy()
    {
        GameManager.Instance.currentPlayerId.OnValueChanged -= SetSelectableState;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelectable) return;
        isSelectable = false;

        OnCardDeckClicked?.Invoke();
        Debug.Log("CardDeck geklickt.");
    }

    private void SetSelectableState(ulong previousPlayerId, ulong currentPlayerId)
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        isSelectable = currentPlayerId == localClientId;
    }
}
