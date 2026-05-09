using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public MapData map;
        public GameObject buttonGO;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI starsText;
        public GameObject lockIcon;
    }

    public LevelButton[] levels;

    void Start()
    {
        int totalStars = PlayerPrefs.GetInt("TotalStars", 0);

        foreach (var level in levels)
        {
            if (level == null || level.map == null || level.buttonGO == null) continue;

            bool unlocked = totalStars >= level.map.starsRequired;
            if (level.lockIcon != null)
                level.lockIcon.SetActive(!unlocked);

            if (level.nameText != null)
                level.nameText.text = level.map.mapName;

            int earned = Mathf.Clamp(PlayerPrefs.GetInt($"Stars_{level.map.mapName}", 0), 0, 3);
            if (level.starsText != null)
                level.starsText.text = new string('★', earned) + new string('☆', 3 - earned);

            var button = level.buttonGO.GetComponent<Button>();
            if (button == null) continue;

            button.interactable = unlocked;
            var map = level.map;
            button.onClick.AddListener(() => LoadMap(map));
        }
    }

    void LoadMap(MapData map)
    {
        if (map == null || string.IsNullOrWhiteSpace(map.sceneName)) return;

        MapLoader.Pending = map;
        SceneManager.LoadScene(map.sceneName);
    }
}
