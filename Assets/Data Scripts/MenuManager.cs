using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }
    public GameObject menuCanvas;
    public GameObject uiCanvas;

    public GameObject saveMenu;
    public GameObject SettingMenu;
    public GameObject mainMenu;

    public bool isMenuOpen;

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
        // Chỉ xử lý khi phím Escape được nhấn
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Nếu menu đang mở, đóng menu lại
            if (isMenuOpen)
            {
                CloseMenu();
            }
            // Nếu menu đang đóng, mở menu lên
            else
            {
                OpenMenu();
            }
        }
    }

    // Phương thức để mở menu
    public void OpenMenu()
    {
        uiCanvas.SetActive(false);
        menuCanvas.SetActive(true);

        // Đảm bảo chỉ có mainMenu được hiển thị khi mở menu
        saveMenu.SetActive(false);
        SettingMenu.SetActive(false);
        mainMenu.SetActive(true);

        isMenuOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SelectionManager.Instance.DisableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
    }

    // Phương thức để đóng menu
    public void CloseMenu()
    {
        uiCanvas.SetActive(true);
        menuCanvas.SetActive(false);
        isMenuOpen = false;

        // Chỉ khóa con trỏ chuột nếu không có hệ thống UI khác đang mở
        if (CraftingSystem.Instance.isOpen == false && InventorySystem.Instance.isOpen == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        SelectionManager.Instance.EnableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
    }
}