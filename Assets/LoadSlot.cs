using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttontext;

    public int slotNumber;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttontext = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (SaveManager.Instance.isSlotEmpty(slotNumber))
        {
            buttontext.text = "";
        }
        else
        {
            buttontext.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }
    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveManager.Instance.isSlotEmpty(slotNumber) == false)
            {
                SaveManager.Instance.StartLoadedGame(slotNumber);
                SaveManager.Instance.DeselectButton();
            }
            else
            {
                // if empty do nothing
            }
        });
    }
}   
