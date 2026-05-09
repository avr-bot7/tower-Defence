using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefence/MapData")]
public class MapData : ScriptableObject
{
    [Header("Identity")]
    public string mapName;
    public string mapDescription;
    public Sprite thumbnail;

    [Header("Gameplay")]
    public int startingGold = 150;
    public int startingLives = 20;
    public int totalWaves = 10;

    [Header("Scene")]
    public string sceneName;

    [Header("Unlock")]
    public int starsRequired;
}
