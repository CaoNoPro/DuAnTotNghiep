using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThirstBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI ThirstCounter;

    public GameObject playerState;
    private float currentThirst, maxThirst;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        currentThirst = playerState.GetComponent<PlayerState>().currentThirst;
        maxThirst = playerState.GetComponent<PlayerState>().maxThirst;

        float fillValue = currentThirst / maxThirst;
        slider.value = fillValue;

        ThirstCounter.text = currentThirst + "%";
    }
}
