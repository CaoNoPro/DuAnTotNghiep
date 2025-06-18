using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }

    public Weapon hoveredWeapon = null; // Reference to the currently hovered weapon
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
        else
        {
            Instance = this;
        }
    }
    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Create a ray from the center of the camera viewport
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Check if the ray hits an object within 3 units
        {
            GameObject objectHitByRayCast = hit.transform.gameObject; // Get the object hit by the raycast

            if (objectHitByRayCast.GetComponent<Weapon>())
            {
                hoveredWeapon = objectHitByRayCast.gameObject.GetComponent<Weapon>(); // If the object has a Weapon component, set it as the hovered weapon
                hoveredWeapon.GetComponent<Outline>().enabled = true; // Enable the outline effect on the hovered weapon

                if (Input.GetKeyDown(KeyCode.F))
                {
                    weaponManager.Instance.PickupWeapon(objectHitByRayCast.gameObject); // Call the PickupWeapon method from weaponManager to handle the interaction 
                }
            }
            else
            {
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false; // Disable the outline effect if the hovered weapon is not hit
                }
            }
        }
    }
}
