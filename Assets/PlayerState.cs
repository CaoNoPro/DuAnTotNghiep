using System.Collections;
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
    Vector3 lastPosition;

    public GameObject playerBody;

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
        distanceTravelled += Vector3.Distance(playerBody.transform.position, lastPosition);
        lastPosition = playerBody.transform.position;

        if (distanceTravelled >= 5)
        {
            distanceTravelled = 0;
            currentHunger -= 1;
        }


        if (Input.GetKeyDown(KeyCode.N))
        {
            currentHealth -= 10;
        }
    }
}
