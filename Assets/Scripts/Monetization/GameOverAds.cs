using TMPro;
using UnityEngine;

public class GameOverAds : MonoBehaviour
{
    [Header("UI")]
    public GameObject reviveButton;
    public TextMeshProUGUI reviveLabel;
    public GameObject gameOverPanel;

    bool _reviveUsed;

    void Start()
    {
        if (reviveLabel != null)
            reviveLabel.text = "Watch ad -> +5 lives";
    }

    public void OnGameOver()
    {
        if (AdsManager.I == null)
        {
            ShowPanelNoAd();
            return;
        }

        AdsManager.I.ShowInterstitial(() =>
        {
            ShowPanelNoAd();
        });
    }

    void ShowPanelNoAd()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (reviveButton != null)
            reviveButton.SetActive(!_reviveUsed);
    }

    public void OnReviveButtonPressed()
    {
        if (AdsManager.I == null)
        {
            Debug.Log("AdsManager not available");
            return;
        }

        AdsManager.I.ShowRewarded(
            onRewarded: () =>
            {
                _reviveUsed = true;
                if (reviveButton != null)
                    reviveButton.SetActive(false);

                if (gameOverPanel != null)
                    gameOverPanel.SetActive(false);

                GameManager.I.Revive(5);
            },
            onFailed: () => Debug.Log("No rewarded ad available for revive")
        );
    }
}
