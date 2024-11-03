using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDeckUI : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnCardDeckClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardDeckClicked?.Invoke();
        Debug.Log("CardDeck geklickt.");
    }
}
