using UnityEngine;
using UnityEngine.UI;

public class PrintButtonHandler : MonoBehaviour
{
    public Button printButton;

    // Start is called before the first frame update
    void Start()
    {
        printButton.onClick.AddListener(OnPrintButtonClicked);
    }

    private void OnPrintButtonClicked()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            gameManager.PrintPlayerDictionary();
        }
        else
        {
            Debug.Log("PlayerManager nicht gefunden");
        }
    }
}
