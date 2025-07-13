using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractiveObjectDisplay : MonoBehaviour
{
    [System.Serializable]
    public class ContentObject
    {
        public Sprite image;
        [TextArea(3, 10)]
        public string content;
        public float displaySpeed = 0.05f;
    }

    [Header("Content Data")]
    public List<ContentObject> objectsToDisplay;

    [Header("UI References")]
    public Image displayImage;
    public TextMeshProUGUI displayText;
    public GameObject startButton;
    public GameObject proceedIndicator;

    private Image proceedFillImage;

    private int currentIndex = 0;
    private bool isTyping = false;
    private bool canProceed = false;
    private bool isHoldingO = false;
    private float holdTimeRequired = 1.5f;
    private float currentHoldTime = 0f;

    void Start()
    {
        if (proceedIndicator != null)
        {
            proceedIndicator.SetActive(false);
            proceedFillImage = proceedIndicator.GetComponentInChildren<Image>();
        }
        else
        {
            Debug.LogWarning("⚠️ proceedIndicator chưa được gán trong Inspector!", this);
        }

        if (displayImage != null)
            displayImage.gameObject.SetActive(false);
        else
            Debug.LogWarning("⚠️ displayImage chưa được gán!", this);

        if (displayText != null)
            displayText.gameObject.SetActive(false);
        else
            Debug.LogWarning("⚠️ displayText chưa được gán!", this);
    }

    public void StartDisplay()
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("❌ Không thể chạy DisplayObject vì GameObject đang bị inactive!", this);
            return;
        }

        if (startButton != null)
            startButton.SetActive(false);
        else
            Debug.LogWarning("⚠️ startButton chưa được gán!", this);

        if (objectsToDisplay == null || objectsToDisplay.Count == 0)
        {
            Debug.LogError("❌ Danh sách objectsToDisplay trống hoặc null!", this);
            return;
        }

        StartCoroutine(DisplayObject(currentIndex));
    }

    IEnumerator DisplayObject(int index)
    {
        if (index >= objectsToDisplay.Count) yield break;

        ContentObject currentObj = objectsToDisplay[index];

        if (displayImage != null)
        {
            displayImage.gameObject.SetActive(true);
            displayImage.sprite = currentObj.image;
        }

        if (displayText != null)
        {
            displayText.gameObject.SetActive(true);
            displayText.text = "";
        }

        isTyping = true;

        foreach (char letter in currentObj.content.ToCharArray())
        {
            if (displayText != null)
                displayText.text += letter;

            yield return new WaitForSeconds(currentObj.displaySpeed);
        }

        isTyping = false;
        canProceed = true;

        if (proceedIndicator != null)
            proceedIndicator.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            isHoldingO = true;
            currentHoldTime = 0f;
        }

        if (Input.GetKey(KeyCode.O) && isHoldingO)
        {
            currentHoldTime += Time.deltaTime;

            if (proceedFillImage != null)
                proceedFillImage.fillAmount = currentHoldTime / holdTimeRequired;

            if (currentHoldTime >= holdTimeRequired && canProceed && !isTyping)
            {
                ProceedToNextObject();
            }
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            isHoldingO = false;

            if (proceedFillImage != null)
                proceedFillImage.fillAmount = 0f;
        }
    }

    void ProceedToNextObject()
    {
        if (displayImage != null)
            displayImage.gameObject.SetActive(false);

        if (displayText != null)
            displayText.gameObject.SetActive(false);

        if (proceedIndicator != null)
            proceedIndicator.SetActive(false);

        canProceed = false;
        currentIndex++;

        if (currentIndex < objectsToDisplay.Count)
        {
            StartCoroutine(DisplayObject(currentIndex));
        }
        else
        {
            if (startButton != null)
                startButton.SetActive(true);

            currentIndex = 0;
        }

        isHoldingO = false;
        currentHoldTime = 0f;
    }
}
