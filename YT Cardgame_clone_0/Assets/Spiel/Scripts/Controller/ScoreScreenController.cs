using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreenController : MonoBehaviour
{
    public GameObject scoreScreenUI;

    public TextMeshProUGUI[] playerName;
    public TextMeshProUGUI[] playerScore;

    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI waitForPlayerText;

    public Button nextRoundButton;
    public static event Action OnNextRoundButtonClickedEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.UpdateScoreScreenEvent += UpdateScoreScreen;
    }

    public void OnDestroy()
    {
        GameManager.UpdateScoreScreenEvent -= UpdateScoreScreen;
    }

    private void UpdateScoreScreen(Player[] players, Player winningPlayer)
    {
        ShowScoreScreen();
        UpdatePlayerPanels(players);
        UpdateWinnerText(winningPlayer);
    }

    private void ShowScoreScreen()
    {
        LeanTween.alphaCanvas(scoreScreenUI.GetComponent<CanvasGroup>(), 1.0f, 1.0f);
        scoreScreenUI.GetComponent<CanvasGroup>().interactable = true;
        scoreScreenUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    private void UpdatePlayerPanels(Player[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            playerName[i].text = players[i].name;
            playerScore[i].text = players[i].score.ToString();
        }
    }

    private void UpdateWinnerText(Player winningPlayer)
    {
        winnerText.text = $"{winningPlayer.name} Gewinnt";
    }

    public void OnNextRoundButtonClicked()
    {
        waitForPlayerText.gameObject.SetActive(true);

        nextRoundButton.interactable = false;

        OnNextRoundButtonClickedEvent?.Invoke();
    }
}
