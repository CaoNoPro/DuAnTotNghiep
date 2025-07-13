using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class StorySlide
{
    public Sprite image;
    [TextArea(2, 10)]
    public string text;
}

public class StoryManager : MonoBehaviour
{
    [Header("UI References")]
    public Image storyImage;
    public TextMeshProUGUI storyText;
    public GameObject storyPanel;
    public GameObject menuPanel;

    [Header("Slides")]
    public List<StorySlide> slides = new List<StorySlide>();
    private int currentIndex = 0;

    void Start()
    {
        storyPanel.SetActive(false);  // Ẩn story lúc đầu
        menuPanel.SetActive(true);    // Hiện menu
    }

    void Update()
    {
        if (storyPanel.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            NextSlide();
        }
    }

    public void StartStory()
    {
        currentIndex = 0;
        menuPanel.SetActive(false);   // Ẩn menu khi bắt đầu
        storyPanel.SetActive(true);   // Hiện story panel
        ShowSlide();
    }

    public void NextSlide()
    {
        currentIndex++;
        if (currentIndex >= slides.Count)
        {
            EndStory();
        }
        else
        {
            ShowSlide();
        }
    }

    void ShowSlide()
    {
        if (slides[currentIndex].image == null)
        {
            Debug.LogWarning("Slide " + currentIndex + " chưa có ảnh.");
        }

        storyImage.sprite = slides[currentIndex].image;
        storyText.text = slides[currentIndex].text;

        Debug.Log("Đang hiển thị ảnh: " + (slides[currentIndex].image != null ? slides[currentIndex].image.name : "null"));
    }

    void EndStory()
    {
        storyPanel.SetActive(false);  // Ẩn story
        menuPanel.SetActive(true);    // Quay lại menu
    }
}
