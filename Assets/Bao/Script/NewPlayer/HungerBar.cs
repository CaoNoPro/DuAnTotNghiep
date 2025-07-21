using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI hungerCounter;

    public GameObject playerState;
    private float currentHunger, maxHunger;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        currentHunger = playerState.GetComponent<PlayerState>().currentHunger;
        maxHunger = playerState.GetComponent<PlayerState>().maxHunger;

        float fillValue = currentHunger / maxHunger;
        slider.value = fillValue;

        hungerCounter.text = currentHunger + "/" + maxHunger;
    }
}
