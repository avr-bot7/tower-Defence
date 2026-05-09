// ============================================================
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Resources")]
    public int gold = 150;
    public int lives = 20;

    [Header("UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    void Awake() => I = this;
    void Start() => RefreshUI();

    public bool SpendGold(int amt)
    {
        if (gold < amt) return false;
        gold -= amt;
        RefreshUI();
        return true;
    }

    public void AddGold(int amt) { gold += amt; RefreshUI(); }

    public void ApplyMapData(MapData data)
    {
        if (data == null) return;

        gold = data.startingGold;
        lives = data.startingLives;

        if (WaveManager.I != null)
            WaveManager.I.totalWaves = data.totalWaves;

        RefreshUI();
    }

    public void LoseLife()
    {
        lives--;
        if (ScreenFlash.I != null)
            ScreenFlash.I.Flash(Color.red);

        RefreshUI();

        if (lives <= 0)
        {
            Time.timeScale = 0;

            var gameOverAds = FindObjectOfType<GameOverAds>();
            if (gameOverAds != null)
                gameOverAds.OnGameOver();
            else if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }

    public void Revive(int livesRestored)
    {
        lives = Mathf.Max(1, livesRestored);
        Time.timeScale = 1f;
        RefreshUI();
    }

    public void Victory()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    void RefreshUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {gold}";

        if (livesText != null)
            livesText.text = $"Lives: {lives}";

        if (TowerBarUI.I != null)
            TowerBarUI.I.RefreshAffordability();
    }
}