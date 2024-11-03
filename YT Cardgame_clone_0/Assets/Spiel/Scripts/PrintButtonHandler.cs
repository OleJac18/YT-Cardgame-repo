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
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager != null)
        {
            playerManager.PrintPlayerDictionary();
        }
        else
        {
            Debug.Log("PlayerManager nicht gefunden");
        }
    }
}
