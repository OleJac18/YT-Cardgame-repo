using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI numberTextTopLeft;
    [SerializeField] private TextMeshProUGUI numberTextBottomRight;
    [SerializeField] private GameObject cardBackImage;
    [SerializeField] private Card _card;

    private Outline _outline;
    private Vector3 _originalScale;
    private Vector3 _hoverScale;

    private void Awake()
    {
        _outline = this.GetComponent<Outline>();
        _originalScale = Vector3.one;
        _hoverScale = new Vector3(1.1f, 1.1f, 1f);
        _card = new Card(13, Card.Stack.NONE);
    }

    public int CardNumber
    {
        get { return _card.number; }
        set
        {
            if (_card.number != value)
            {
                _card.number = value;
                UpdateCardNumber();
            }
        }
    }

    private void UpdateCardNumber()
    {
        string cardNumber = _card.number.ToString();
        numberTextTopLeft.text = cardNumber;
        numberTextBottomRight.text = cardNumber;
    }

    public void SetCorrespondingDeck(Card.Stack decktype)
    {
        _card.correspondingDeck = decktype;
    }

    public void SetCardBackImageVisibility(bool visible)
    {
        cardBackImage.SetActive(visible);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Ich habe die Maus auf die Karte bewegt.");

        this.transform.localScale = _hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Ich habe die Maus von der Karte herunter bewegt.");

        this.transform.localScale = _originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Ich habe die Karte angeklickt.");
        //SelectionAnimation();
        FlipCardAnimation();
    }

    private void SelectionAnimation()
    {
        if (_outline == null)
        {
            Debug.Log("Das Object " + name + " hat keine Komponente Outline");
            return;
        }

        _outline.enabled = !_outline.enabled;
    }

    private void FlipCardAnimation()
    {
        LeanTween.rotateY(this.gameObject, 90.0f, 0.25f).setOnComplete(() =>
        {
            cardBackImage.SetActive(!cardBackImage.activeSelf);
            LeanTween.rotateY(this.gameObject, 0.0f, 0.25f);
        });
    }
}
