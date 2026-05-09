using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefence/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;
        public int count;
        public float spawnDelay;
    }

    public SpawnEntry[] entries;
    public bool isBossWave;
    public string waveTitle;
}
