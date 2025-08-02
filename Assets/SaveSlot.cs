using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttontext;

    public int slotNumber;

    public GameObject alertUI;
    Button yesBTN;
    Button noBTN;
    private void Awake()
    {
        button = GetComponent<Button>();
        buttontext = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        yesBTN = alertUI.transform.Find("YesButton").GetComponent<Button>();
        noBTN = alertUI.transform.Find("NoButton").GetComponent<Button>();
    }

    public void Start()
    {
        button.onClick.AddListener(() =>
        {
            SaveManager.Instance.SaveGame(slotNumber);
            if (SaveManager.Instance.isSlotEmpty(slotNumber))
            {
                SaveGameConfirmed();
            }
            else
            {
                DisplayOverrideWarning();
            }
        });
    }

    private void Update()
    {
        if (SaveManager.Instance.isSlotEmpty(slotNumber))
        {
            buttontext.text = "Empty";
        }
        else
        {
            buttontext.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }

    public void DisplayOverrideWarning()
    {
        alertUI.SetActive(true);

        yesBTN.onClick.AddListener(() =>
        {
            SaveGameConfirmed();

            alertUI.SetActive(false);
        });

        noBTN.onClick.AddListener(() =>
        {
            alertUI.SetActive(false);
        });
    }

    private void SaveGameConfirmed()
    {
        DateTime dt = DateTime.Now;
        string time = dt.ToString("yyyy-MM-dd HH:mm");
        string description = "Saved Game" + slotNumber + "|" + time;

        buttontext.text = description;

        PlayerPrefs.SetString("Slot" + slotNumber + "Description", description);

        SaveManager.Instance.DeselectButton();
    }
}
