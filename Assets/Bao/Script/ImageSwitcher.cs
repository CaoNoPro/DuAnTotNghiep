using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ImageSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class ImageData
    {
        public Sprite image;
        public string description;
        public GameObject model3D;
    }

    [Header("Image List")]
    public List<ImageData> imageList = new List<ImageData>();

    [Header("UI References")]
    public Image displayImage;
    public TextMeshProUGUI descriptionText;

    [Header("Animation Settings")]
    public float scaleDuration = 0.3f;
    public float maxScale = 1.2f;

    private int currentIndex = 0;
    private Coroutine scaleCoroutine;

    void Start() => ShowImage(0);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ShowImage(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ShowImage(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ShowImage(2);
    }

    public void ShowImage(int index)
    {
        if (index < 0 || index >= imageList.Count) return;

        // Tắt model cũ
        if (currentIndex < imageList.Count && imageList[currentIndex].model3D != null)
            imageList[currentIndex].model3D.SetActive(false);

        currentIndex = index;

        // Cập nhật UI
        displayImage.sprite = imageList[currentIndex].image;
        descriptionText.text = imageList[currentIndex].description;

        // Bật model mới
        if (imageList[currentIndex].model3D != null)
            imageList[currentIndex].model3D.SetActive(true);

        PlaySwitchEffect();
    }

    void PlaySwitchEffect()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
            
        scaleCoroutine = StartCoroutine(ScaleEffect());
    }

    IEnumerator ScaleEffect()
    {
        float elapsed = 0f;
        Transform t = displayImage.transform;
        Vector3 startScale = Vector3.one * maxScale;
        Vector3 endScale = Vector3.one;

        while (elapsed < scaleDuration)
        {
            t.localScale = Vector3.Lerp(startScale, endScale, elapsed/scaleDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        t.localScale = endScale;
    }
}
