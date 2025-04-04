using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private Image activePlayerImage;

    private void Start()
    {
        PlayerUIManager.InitializePlayerUIEvent += Initialize;
    }

    private void OnDestroy()
    {
        PlayerUIManager.InitializePlayerUIEvent -= Initialize;
    }

    private void Initialize(string playerName, int playerScore, bool isCurrentPlayer)
    {
        playerNameText.text = playerName;
        UpdateScore(playerScore);
        SetActivePlayer(isCurrentPlayer);
    }

    public void UpdateScore(int score)
    {
        playerScoreText.text = "Score: " + score;
    }

    public void SetActivePlayer(bool isCurrentPlayer)
    {
        activePlayerImage.color = isCurrentPlayer ? Color.green : Color.grey;
    }
}
