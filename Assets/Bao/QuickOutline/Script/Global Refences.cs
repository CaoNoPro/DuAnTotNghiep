using UnityEngine;

public class GlobalRefences : MonoBehaviour
{

    public static GlobalRefences Instance { get; set; }

    public GameObject bulletImpactEffectPrefab; // Reference to the bullet impact effect prefab

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevents the object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
    }
}
