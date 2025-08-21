using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    // Health
    public float currentHealth;
    public float maxHealth;

    //Thirst
    public float currentThirst;
    public float maxThirst;

    public bool isThirstActive;

    //Hunger
    public float currentHunger;
    public float maxHunger;
    float distanceTravelled = 0;
    public Vector3 lastPosition;

    public GameObject playerBody;
    public GameObject diePanel;
    public TextMeshProUGUI loadingText;
    public bool isDead = false;

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
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        StartCoroutine(decreaseThirst());

        if (diePanel != null)
        {
            diePanel.SetActive(false);
        }
        lastPosition = playerBody.transform.position;
    }

    IEnumerator decreaseThirst()
    {
        while (true)
        {
            currentThirst -= 1;
            yield return new WaitForSeconds(10);
        }
    }

    void Update()
    {
        if (!isDead)
        {
            distanceTravelled += Vector3.Distance(playerBody.transform.position, lastPosition);
            lastPosition = playerBody.transform.position;

            if (distanceTravelled >= 5)
            {
                distanceTravelled = 0;
                currentHunger -= 1;
            }
        }
    }

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public void setThirst(float newThirst)
    {
        currentThirst = newThirst;
    }

    public void setHunger(float newHunger)
    {
        currentHunger = newHunger;
    }

    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
                Debug.Log("Player is Dead");

                if (diePanel != null)
                {
                    diePanel.SetActive(true);
                }

                if (SaveManager.Instance != null && SaveManager.Instance.DoesFileExists(1))
                {
                    Debug.Log("Auto-loading last save file...");
                    SaveManager.Instance.StartLoadedGame(1);
                }
                else
                {
                    Debug.LogWarning("No save file found. Cannot auto-load.");
                }
            }
            else
            {
                Debug.Log("player get hit");
            }
        }
    }
}