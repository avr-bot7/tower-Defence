using System.Collections;
using TMPro;
using UnityEngine;

public class StarRatingSystem : MonoBehaviour
{
    [Header("Thresholds (lives remaining)")]
    public int threeStar = 18;
    public int twoStar = 12;

    [Header("UI")]
    public GameObject resultsPanel;
    public GameObject[] starObjects;
    public TextMeshProUGUI resultTitle;
    public TextMeshProUGUI goldEarnedText;

    int _goldEarned;

    public void TrackGold(int gold)
    {
        _goldEarned += gold;
    }

    public void ShowResults(int livesRemaining, string mapName)
    {
        int stars = livesRemaining >= threeStar ? 3 : livesRemaining >= twoStar ? 2 : 1;

        int previous = PlayerPrefs.GetInt($"Stars_{mapName}", 0);
        if (stars > previous)
        {
            PlayerPrefs.SetInt($"Stars_{mapName}", stars);
            PlayerPrefs.SetInt("TotalStars", PlayerPrefs.GetInt("TotalStars", 0) + (stars - previous));
            PlayerPrefs.Save();
        }

        if (resultsPanel != null)
            resultsPanel.SetActive(true);

        if (resultTitle != null)
            resultTitle.text = stars == 3 ? "Perfect Victory!" : stars == 2 ? "Victory!" : "Close Call!";

        if (goldEarnedText != null)
            goldEarnedText.text = $"Gold Earned: {_goldEarned}";

        for (int i = 0; i < starObjects.Length; i++)
            if (starObjects[i] != null)
                starObjects[i].SetActive(i < stars);

        StartCoroutine(AnimateStars(stars));
    }

    IEnumerator AnimateStars(int count)
    {
        for (int i = 0; i < count && i < starObjects.Length; i++)
        {
            var star = starObjects[i];
            if (star == null) continue;

            yield return new WaitForSeconds(0.35f);

            star.SetActive(true);
            var startScale = Vector3.one;
            var peakScale = Vector3.one * 1.25f;
            float t = 0f;

            while (t < 0.15f)
            {
                t += Time.unscaledDeltaTime;
                star.transform.localScale = Vector3.Lerp(startScale, peakScale, t / 0.15f);
                yield return null;
            }

            t = 0f;
            while (t < 0.1f)
            {
                t += Time.unscaledDeltaTime;
                star.transform.localScale = Vector3.Lerp(peakScale, startScale, t / 0.1f);
                yield return null;
            }

            star.transform.localScale = startScale;
        }
    }
}
