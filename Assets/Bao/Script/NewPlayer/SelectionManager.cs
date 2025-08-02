using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }
    public bool onTarget;
    public GameObject interaction_info_ui;
    TextMeshProUGUI interactionText;

    public Image centerPoint;
    public Image hanIcon;

    private void Start()
    {
        onTarget = false;
        interactionText = interaction_info_ui.GetComponent<TextMeshProUGUI>();
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; 
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            InteractableObject Interactable = selectionTransform.GetComponent<InteractableObject>();

            if (Interactable && Interactable.playerInRange)
            {
                onTarget = true;

                interactionText.text = Interactable.GetItemName();
                interaction_info_ui.SetActive(true);

                if (Interactable.CompareTag("pickable"))
                {
                    centerPoint.gameObject.SetActive(false);
                    hanIcon.gameObject.SetActive(true);
                }
                else
                {
                    hanIcon.gameObject.SetActive(false);
                    centerPoint.gameObject.SetActive(true);
                }
            }
            else
            {
                onTarget = false;
                interaction_info_ui.SetActive(false);
                hanIcon.gameObject.SetActive(false);
                centerPoint.gameObject.SetActive(true);
            }
        }
        else
        {
            onTarget = false;
            interaction_info_ui.SetActive(false);
            hanIcon.gameObject.SetActive(false);
            centerPoint.gameObject.SetActive(true);
        }
    }

    public void DisableSelection()
    {
        hanIcon.enabled = false;
        centerPoint.enabled = false;
        interaction_info_ui.SetActive(false);

    }

    public void EnableSelection()
    {
        hanIcon.enabled = true;
        centerPoint.enabled = true;
        interaction_info_ui.SetActive(true);


    }
}
