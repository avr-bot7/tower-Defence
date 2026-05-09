using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager I;

    public Button startWaveButton;

    [Header("Wave Data (optional - use ScriptableObjects)")]
    public WaveData[] waveDatas;

    [Header("Spawning")]
    public GameObject[] enemyPrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 0.8f;

    [Header("Wave Config")]
    public int baseEnemyCount = 8;
    public int enemiesPerWave = 4;
    public int totalWaves = 10;

    [Header("Auto Wave")]
    public bool autoStartWhenFewEnemiesRemain = true;
    public int autoStartThreshold = 2;
    public float autoStartDelay = 1f;

    int _currentWave;
    int _aliveCount;
    bool _waveActive;
    bool _autoWaveQueued;
    int _queuedWaveStarts;

    void Awake()
    {
        I = this;
    }

    public void StartNextWave()
    {
        if (_waveActive)
        {
            if (_currentWave + _queuedWaveStarts < totalWaves)
                _queuedWaveStarts++;
            return;
        }

        _autoWaveQueued = false;
        _currentWave++;
        if (_currentWave > totalWaves) { GameManager.I.Victory(); return; }

        int count = GetEnemyCount();

        // Only call WaveUIManager if it exists in scene
        if (WaveUIManager.I != null)
            WaveUIManager.I.OnWaveStarting(_currentWave, totalWaves, count);

        StartCoroutine(SpawnWave(count));   // spawn directly, no countdown wait
    }

    IEnumerator WaitThenSpawn(int count)
    {
        float wait = WaveUIManager.I != null ? WaveUIManager.I.countdownTime + 0.6f : 0f;
        yield return new WaitForSeconds(wait);
        StartCoroutine(SpawnWave(count));
    }

    IEnumerator SpawnWave(int count)
    {
        _waveActive = true;

        if (waveDatas != null && _currentWave <= waveDatas.Length && waveDatas[_currentWave - 1] != null)
        {
            var data = waveDatas[_currentWave - 1];
            foreach (var entry in data.entries)
            {
                if (entry == null || entry.prefab == null) continue;

                yield return new WaitForSeconds(entry.spawnDelay);

                for (int i = 0; i < entry.count; i++)
                {
                    Instantiate(entry.prefab, spawnPoint.position, Quaternion.identity);
                    _aliveCount++;
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                if (enemyPrefabs == null || enemyPrefabs.Length == 0) break;

                int maxType = Mathf.Min(_currentWave / 3, enemyPrefabs.Length - 1);
                int type = Random.Range(0, maxType + 1);
                var e = Instantiate(enemyPrefabs[type], spawnPoint.position, Quaternion.identity);
                _aliveCount++;
                e.GetComponent<Enemy>().speed = Random.Range(3f, 9f);

                var enemy = e.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.maxHP *= 1f + (_currentWave - 1) * 0.15f;

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        _waveActive = false;

        if (_queuedWaveStarts > 0)
        {
            _queuedWaveStarts--;
            StartNextWave();
        }
    }

    public void OnEnemyRemoved()
    {
        _aliveCount = Mathf.Max(0, _aliveCount - 1);

        if (WaveUIManager.I != null) WaveUIManager.I.OnEnemyDefeated();

        bool canAutoStart =
            autoStartWhenFewEnemiesRemain &&
            !_waveActive &&
            !_autoWaveQueued &&
            _currentWave < totalWaves &&
            _aliveCount <= autoStartThreshold;

        if (canAutoStart)
        {
            _autoWaveQueued = true;
            StartCoroutine(AutoStartNextWave());
        }

        if (_aliveCount <= 0 && !_waveActive)
        {
            if (startWaveButton != null)
                startWaveButton.gameObject.SetActive(true);

            if (WaveUIManager.I != null)
                WaveUIManager.I.OnWaveCleared(_currentWave + 1, totalWaves);
        }
    }

    IEnumerator AutoStartNextWave()
    {
        yield return new WaitForSeconds(autoStartDelay);

        if (_waveActive) { _autoWaveQueued = false; yield break; }
        if (_currentWave >= totalWaves) { _autoWaveQueued = false; yield break; }
        if (_aliveCount > autoStartThreshold) { _autoWaveQueued = false; yield break; }

        StartNextWave();
    }

    int GetEnemyCount()
    {
        if (waveDatas != null && _currentWave <= waveDatas.Length && waveDatas[_currentWave - 1] != null)
        {
            int total = 0;
            foreach (var entry in waveDatas[_currentWave - 1].entries)
            {
                if (entry != null)
                    total += entry.count;
            }
            return total;
        }

        return baseEnemyCount + (_currentWave - 1) * enemiesPerWave;
    }

    string WaveFlavourText(int wave)
    {
        return wave switch
        {
            1 => "The First Asuras March",
            2 => "Darkness Grows",
            3 => "The Armoured Horde",
            4 => "Vimanas Take Flight",
            5 => "Midpoint - Hold the Line",
            6 => "The Sky Darkens",
            7 => "Rakshasa Legion",
            8 => "The Ground Shakes",
            9 => "Last Stand",
            10 => "Mahishasura Arrives",
            _ => $"Wave {wave}"
        };
    }
}