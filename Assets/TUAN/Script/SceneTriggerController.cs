using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneTriggerController : MonoBehaviour
{
    [Header("Visual Cue")]
    [Tooltip("UI thông báo cho người chơi biết có thể tương tác.")]
    [SerializeField] private GameObject interactionPromptUI; 

    [Header("Scene Configuration")]
    [Tooltip("Tên của scene sẽ được tải.")]
    [SerializeField] private string sceneToLoad; 


    private bool isPlayerInRange = false;

    private void Start()
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            // Nếu người chơi nhấn phím F
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Tải scene đã được chỉ định
                LoadTargetScene();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // Bật UI lên để thông báo
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // Tắt UI đi
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(false);
            }
        }
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Tên scene cần tải chưa được thiết lập trong Inspector!");
        }
    }
}