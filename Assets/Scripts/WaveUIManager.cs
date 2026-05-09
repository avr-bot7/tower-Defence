using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveUIManager : MonoBehaviour
{
    public static WaveUIManager I;

    [Header("Wave Info")]
    public TextMeshProUGUI waveLabel;
    public TextMeshProUGUI waveSubtitle;

    [Header("Countdown")]
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownNumber;
    public float countdownTime = 3f;

    [Header("Progress Bar")]
    public Slider waveProgressBar;
    public TextMeshProUGUI progressLabel;

    [Header("Announcement Banner")]
    public GameObject bannerPanel;
    public TextMeshProUGUI bannerTitle;
    public TextMeshProUGUI bannerSub;
    public float bannerDuration = 2f;
    public Animator bannerAnimator;

    [Header("Next Wave Button")]
    public GameObject nextWaveBtn;
    public TextMeshProUGUI nextWaveBtnLabel;

    [Header("Boss Warning")]
    public GameObject bossWarningPanel;
    public float bossWarningDuration = 3f;

    int _totalEnemies;
    int _defeatedEnemies;

    void Awake()
    {
        I = this;
    }

    public void OnWaveStarting(int waveNum, int totalWaves, int enemyCount, string subtitle = "")
    {
        _totalEnemies = enemyCount;
        _defeatedEnemies = 0;

        if (waveLabel)
            waveLabel.text = $"Wave {waveNum} / {totalWaves}";

        if (waveSubtitle)
            waveSubtitle.text = subtitle;

        RefreshProgress();

        StartCoroutine(CountdownThenBanner(waveNum, subtitle));
    }

    public void ShowBossWarning(string bossName)
    {
        StartCoroutine(BossWarningRoutine(bossName));
    }

    public void OnEnemyDefeated()
    {
        _defeatedEnemies++;
        RefreshProgress();
    }

    public void OnWaveCleared(int nextWaveNum, int totalWaves)
    {
        bool lastWave = nextWaveNum > totalWaves;
        if (nextWaveBtn)
            nextWaveBtn.SetActive(!lastWave);

        if (!lastWave && nextWaveBtnLabel)
            nextWaveBtnLabel.text = $"Start Wave {nextWaveNum} >";
    }

    void RefreshProgress()
    {
        float t = _totalEnemies > 0 ? (float)_defeatedEnemies / _totalEnemies : 0f;

        if (waveProgressBar)
            waveProgressBar.value = t;

        if (progressLabel)
            progressLabel.text = $"{_defeatedEnemies} / {_totalEnemies} defeated";
    }

    IEnumerator CountdownThenBanner(int waveNum, string subtitle)
    {
        if (countdownPanel)
            countdownPanel.SetActive(true);

        float remaining = countdownTime;
        while (remaining > 0f)
        {
            if (countdownNumber)
            {
                countdownNumber.text = Mathf.CeilToInt(remaining).ToString();
                StartCoroutine(PulseCountdown());
            }

            remaining -= 1f;
            yield return new WaitForSeconds(1f);
        }

        if (countdownNumber)
            countdownNumber.text = "GO!";

        yield return new WaitForSeconds(0.5f);

        if (countdownPanel)
            countdownPanel.SetActive(false);

        ShowBanner($"WAVE {waveNum}", subtitle);
    }

    IEnumerator PulseCountdown()
    {
        if (countdownNumber == null) yield break;

        Transform tr = countdownNumber.transform;
        Vector3 start = Vector3.one * 1.4f;
        Vector3 end = Vector3.one;
        float elapsed = 0f;
        const float duration = 0.35f;

        tr.localScale = start;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            tr.localScale = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        tr.localScale = end;
    }

    void ShowBanner(string title, string sub)
    {
        if (bannerTitle)
            bannerTitle.text = title;

        if (bannerSub)
            bannerSub.text = sub;

        if (bannerPanel)
            bannerPanel.SetActive(true);

        if (bannerAnimator)
            bannerAnimator.SetTrigger("Show");

        StartCoroutine(HideBannerAfter(bannerDuration));
    }

    IEnumerator HideBannerAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bannerAnimator)
            bannerAnimator.SetTrigger("Hide");

        yield return new WaitForSeconds(0.4f);

        if (bannerPanel)
            bannerPanel.SetActive(false);
    }

    IEnumerator BossWarningRoutine(string bossName)
    {
        if (bossWarningPanel)
            bossWarningPanel.SetActive(true);

        var text = bossWarningPanel ? bossWarningPanel.GetComponentInChildren<TextMeshProUGUI>() : null;
        if (text)
            text.text = $"{bossName.ToUpper()} APPROACHES";

        StartCoroutine(ScreenShake(0.3f, bossWarningDuration));

        yield return new WaitForSeconds(bossWarningDuration);

        if (bossWarningPanel)
            bossWarningPanel.SetActive(false);
    }

    IEnumerator ScreenShake(float intensity, float duration)
    {
        var cam = Camera.main;
        if (cam == null) yield break;

        var originalPosition = cam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float z = Random.Range(-1f, 1f) * intensity;
            cam.transform.localPosition = originalPosition + new Vector3(x, 0f, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPosition;
    }
}
