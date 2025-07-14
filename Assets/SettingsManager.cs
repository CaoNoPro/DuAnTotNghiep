using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static MainMenuSaveManager;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; set; }
    public Button backBTN;

    public Slider masterSlider;
    public GameObject masterValue;
    public Slider musicSlider;
    public GameObject musicValue;
    public Slider EffectsSlider;
    public GameObject EffectsValue;

    private void Start()
    {
        backBTN.onClick.AddListener(() =>
        {
            MainMenuSaveManager.Instance.SaveVolumeSetting(musicSlider.value, EffectsSlider.value, masterSlider.value);

            print("Saved to player pref");
        });

        StartCoroutine(LoadAndApplySettings());
    }

    private IEnumerator LoadAndApplySettings()
    {
        LoadAndSetVolume();
        yield return new WaitForSeconds(0.1f);
    }
    private void LoadAndSetVolume()
    {
        VolumeSetting volumeSetting = MainMenuSaveManager.Instance.LoadVolumeSetting();

        masterSlider.value = volumeSetting.master;
        musicSlider.value = volumeSetting.music;
        EffectsSlider.value = volumeSetting.effects;

        print("volume Settings are loaded"); 
    }
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

    void Update()
    {
        masterValue.GetComponent<TextMeshProUGUI>().text = "" + (masterSlider.value) + "";
        musicValue.GetComponent<TextMeshProUGUI>().text = "" + (musicSlider.value) + "";
        EffectsValue.GetComponent<TextMeshProUGUI>().text = "" + (EffectsSlider.value) + "";
    }
}
