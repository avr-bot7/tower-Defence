using TMPro;
using UnityEngine;

public class RewardedGoldButton : MonoBehaviour
{
    public int goldReward = 50;
    public TextMeshProUGUI label;

    bool _usedThisWave;

    void Start()
    {
        if (label != null)
            label.text = $"> Watch Ad -> +{goldReward} Gold";
    }

    public void OnWaveCleared()
    {
        _usedThisWave = false;
        gameObject.SetActive(true);
    }

    public void OnPressed()
    {
        if (_usedThisWave || AdsManager.I == null) return;

        AdsManager.I.ShowRewarded(
            onRewarded: () =>
            {
                GameManager.I.AddGold(goldReward);
                _usedThisWave = true;
                gameObject.SetActive(false);
            },
            onFailed: () => Debug.Log("Rewarded ad unavailable")
        );
    }
}
