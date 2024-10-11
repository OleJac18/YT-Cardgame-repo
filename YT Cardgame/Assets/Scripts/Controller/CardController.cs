using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI numberTextTopLeft;
    [SerializeField] private TextMeshProUGUI numberTextBottomRight;

    private Outline _outline;
    private Vector3 _originalScale;
    private Vector3 _hoverScale;

    private void Awake()
    {
        _outline = this.GetComponent<Outline>();
        _originalScale = Vector3.one;
        _hoverScale = new Vector3(1.1f, 1.1f, 1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        numberTextTopLeft.text = "1";
        numberTextBottomRight.text = "1";
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
        SelectionAnimation();
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
}
