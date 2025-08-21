using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public bool playerInRange;

    public bool isTalkingWithPlayer;

    TextMeshProUGUI npcDialogText;

    Button optionBTN1;
    TextMeshProUGUI optionBTN1Text;

    Button optionBTN2;
    TextMeshProUGUI optionBTN2Text;

    public List<Quest> quests;
    public Quest currentActiveQuest = null;
    public int activeQuestIndex = 0;
    public bool fistTimeInteraction = true;
    public int currentDialog;

    void Start()
    {
        npcDialogText = DialogSystem.Instance.dialogText;
        optionBTN1 = DialogSystem.Instance.option1BTN;
        optionBTN1Text = DialogSystem.Instance.option1BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        
        optionBTN2 = DialogSystem.Instance.option2BTN;
        optionBTN2Text = DialogSystem.Instance.option2BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    public void StartConversation()
    {
        isTalkingWithPlayer = true;
        LookAtPlayer();

        DialogSystem.Instance.OpenDialogUI();
        DialogSystem.Instance.dialogText.text = "Hello there";
        DialogSystem.Instance.option1BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Bye";
        DialogSystem.Instance.option1BTN.onClick.AddListener(() =>
        {
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        });
        // if (fistTimeInteraction)
        // {
        //     fistTimeInteraction = false;
        //     currentActiveQuest = quests[activeQuestIndex];
        //     StartQuestInitialDialog();
        //     currentDialog = 0;
        // }

    }

    private void StartQuestInitialDialog()
    {
        DialogSystem.Instance.OpenDialogUI();

        // npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
        optionBTN1Text.text = "Next";
        optionBTN1.onClick.RemoveAllListeners();
        optionBTN1.onClick.AddListener(() =>
        {
            currentDialog++;
            checkIfDialogDone();
        });

        optionBTN2.gameObject.SetActive(false);
    }

    private void checkIfDialogDone()
    {
        throw new NotImplementedException();
    }

    public void LookAtPlayer()
    {
        var player = PlayerState.Instance.playerBody.transform;
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);

        var YRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0,YRotation,0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

}
