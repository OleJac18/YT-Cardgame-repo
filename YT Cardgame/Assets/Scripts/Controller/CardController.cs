using TMPro;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberTextTopLeft;
    [SerializeField] private TextMeshProUGUI numberTextBottomRight;

    // Start is called before the first frame update
    void Start()
    {
        numberTextTopLeft.text = "1";
        numberTextBottomRight.text = "1";
    }
}
