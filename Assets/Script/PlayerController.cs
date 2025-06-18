using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float Gravity = -9.81f;
    public float jumpHeight = 2f;
    public float Health = 100f;
    public float regenRate = 2f;
    public float pickupRange = 3f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public Transform cameraTransform;
    public Transform weaponHolder;
    public Gun[] weapons;
    private int currentWeaponIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        EquipWeapon(currentWeaponIndex);
        InvokeRepeating("RegenerateHealth", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // Reset vertical velocity when grounded
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Gravity);
        }
        velocity.y += Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Length > 1)
        {
            EquipWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Length > 2)
        {
            EquipWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupWeapon();
        }
    }
    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == index);
        }
        currentWeaponIndex = index;
    }
    void AutoRegenHealth()
    {
        if (Health < 100f)
        {
            Health += regenRate * Time.deltaTime;
            Health = Mathf.Min(Health, 100f); // Cap health at 100
        }
    }
    private void TryPickupWeapon()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (Collider hit in hits)
        {
            PickupWeapon pickup = hit.GetComponent<PickupWeapon>();
            if (pickup != null)
            {
                GameObject newWeaponObj = Instantiate(pickup.weaponPrefab, weaponHolder);
                newWeaponObj.transform.localPosition = Vector3.zero;
                newWeaponObj.transform.localRotation = Quaternion.identity;

                Gun newGun = newWeaponObj.GetComponent<Gun>();
                if (newGun != null)
                {
                    // Add to weapon list
                    Gun[] newWeapons = new Gun[weapons.Length + 1];
                    for (int i = 0; i < weapons.Length; i++)
                    {
                        newWeapons[i] = weapons[i];
                    }
                    newWeapons[weapons.Length] = newGun;
                    weapons = newWeapons;

                    EquipWeapon(weapons.Length - 1);
                }
                Destroy(hit.gameObject);
                break;
            }
        }
    }
}
