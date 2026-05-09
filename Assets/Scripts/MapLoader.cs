using System.Collections;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public static MapData Pending;

    void Start()
    {
        if (Pending == null)
            return;

        StartCoroutine(ApplyWhenReady(Pending));
        Pending = null;
    }

    IEnumerator ApplyWhenReady(MapData data)
    {
        while (GameManager.I == null)
            yield return null;

        GameManager.I.ApplyMapData(data);

        if (WaveManager.I != null)
            WaveManager.I.totalWaves = data.totalWaves;
    }
}
