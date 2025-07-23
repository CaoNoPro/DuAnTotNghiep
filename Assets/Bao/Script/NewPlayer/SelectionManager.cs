using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }
    public bool onTarget;
    public GameObject interaction_info_ui;
    TextMeshProUGUI interactionText;

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
            }
            else
            {
                onTarget = false;
                interaction_info_ui.SetActive(false);
            }
        }
        else
        {
            onTarget = false;
            interaction_info_ui.SetActive(false);
        }
    }
}
