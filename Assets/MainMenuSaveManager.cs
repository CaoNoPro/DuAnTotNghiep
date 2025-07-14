using UnityEngine;
using UnityEngine.Rendering;

public class MainMenuSaveManager : MonoBehaviour
{
    public static MainMenuSaveManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [System.Serializable]
    public class VolumeSetting
    {
        public float music;
        public float effects;
        public float master;
    }
    public void SaveVolumeSetting(float _music, float _effects, float _master)
    {
        VolumeSetting volumeSetting = new VolumeSetting()
        {
            music = _music,
            effects = _effects,
            master = _master
        };
        PlayerPrefs.SetString("volume", JsonUtility.ToJson(volumeSetting));
        PlayerPrefs.Save();

    }

    public VolumeSetting LoadVolumeSetting()
    {
        return JsonUtility.FromJson<VolumeSetting>(PlayerPrefs.GetString("volume"));
    }
    public float LoadMusic()
    {
        var volumeSetting = JsonUtility.FromJson<VolumeSetting>(PlayerPrefs.GetString("volume"));
        return volumeSetting.music;
    }
}

