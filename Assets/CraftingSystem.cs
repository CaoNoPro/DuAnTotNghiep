using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingSceenUI;
    public GameObject MedicUI;

    public List<string> inventoryItemList = new List<string>();

    //Category
    Button medicBTN;

    //Crafting BTN
    Button craftMedicBTN;

    //Rep Text
    TextMeshProUGUI MedicReq1, MedicReq2;

    public bool isOpen;

    //All BluePrint
    public BluePrint MedkitBLP = new BluePrint("Medkit", 2, "Cloth", 2, "Alcohol", 1);


    public static CraftingSystem Instance { get; set; }

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

    void Start()
    {
        isOpen = false;
        medicBTN = craftingSceenUI.transform.Find("MedkitButton").GetComponent<Button>();
        medicBTN.onClick.AddListener(delegate { OpenMedicCategory(); });

        //Medkit
        MedicReq1 = MedicUI.transform.Find("Medkit").transform.Find("MedReq1").GetComponent<TextMeshProUGUI>();
        MedicReq2 = MedicUI.transform.Find("Medkit").transform.Find("MedReq2").GetComponent<TextMeshProUGUI>();

        craftMedicBTN = MedicUI.transform.Find("Medkit").transform.Find("Button").GetComponent<Button>();
        craftMedicBTN.onClick.AddListener(delegate { CraftAnyItem(MedkitBLP); });

    }

    void OpenMedicCategory()
    {
        craftingSceenUI.SetActive(false);
        MedicUI.SetActive(true);
    }

    void CraftAnyItem(BluePrint bluePrintToCraft)
    {
        //đặt vật phẩm vào túi đồ

        InventorySystem.Instance.AddToInventory(bluePrintToCraft.itemName);

        //xóa vật phẩm trong túi đồ

        if (bluePrintToCraft.numOfReq == 1)
        {
            InventorySystem.Instance.RemoveItem(bluePrintToCraft.Req1, bluePrintToCraft.Req1amount);
        }
        else if (bluePrintToCraft.numOfReq == 2)
        {
            InventorySystem.Instance.RemoveItem(bluePrintToCraft.Req1, bluePrintToCraft.Req1amount);
            InventorySystem.Instance.RemoveItem(bluePrintToCraft.Req2, bluePrintToCraft.Req2amount);
        }


        StartCoroutine(calulate());

        RefreshNeededItems();

    }

    public IEnumerator calulate()
    {
        yield return new WaitForSeconds(1f);
        InventorySystem.Instance.ReCalculateList();
    }
    void Update()
    {
        RefreshNeededItems();

        if (Input.GetKeyDown(KeyCode.L) && !isOpen)
        {

            Debug.Log("l is pressed");
            craftingSceenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.L) && isOpen)
        {
            craftingSceenUI.SetActive(false);
            MedicUI.SetActive(false);

            if (InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            isOpen = false;
        }
    }

    public void RefreshNeededItems()
    {

        int Cloth_count = 0;
        int Alcohol_count = 0;

        inventoryItemList = InventorySystem.Instance.itemList;

        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Cloth":
                    Cloth_count += 1;
                    break;
                case "Alcohol":
                    Alcohol_count += 1;
                    break;
            }
        }



        //------medkit-------//

        MedicReq1.text = "2 Cloth[" + Cloth_count + "]";
        MedicReq2.text = "1 Alcohol[" + Alcohol_count + "]";

        if (Cloth_count >= 2 && Alcohol_count >= 1)
        {
            craftMedicBTN.gameObject.SetActive(true);
        }
        else
        {
            craftMedicBTN.gameObject.SetActive(false);
        }
    } 
}
