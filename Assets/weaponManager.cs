using TMPro;
using UnityEngine;


public class weaponManager : MonoBehaviour
{
    [Header("References")]
    public Animator animator; // Animator của nhân vật
    public GameObject[] weapons; // Danh sách các súng
    public GameObject bulletPrefab; // Prefab của đạn
    public Transform bulletSpawnPoint; // Điểm xuất phát của đạn
    public TextMeshProUGUI ammo;

    [Header("Weapon Settings")]
    public int[] maxAmmoCounts; // Số đạn tối đa cho mỗi súng
    public float[] weaponDamage; // Damage value for each weapon (NEW!)
    private int[] ammoCounts; // Số đạn hiện tại cho mỗi súng

    private int currentWeaponIndex = 0; // Chỉ số súng hiện tại

    private void Start()
    {
        // Khởi tạo số đạn cho mỗi súng
        ammoCounts = new int[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == currentWeaponIndex);
            ammoCounts[i] = maxAmmoCounts[i]; // Khởi tạo số đạn cho mỗi súng
        }
        Updateammo();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Đổi súng
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }

        // Nạp đạn
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        // Bắn đạn
        if (Input.GetButtonDown("Fire1")) // Thay đổi theo input của bạn
        {
            Shoot();
        }
    }

    private void SwitchWeapon(int index)
    {
        // Kiểm tra chỉ số hợp lệ
        if (index < 0 || index >= weapons.Length)
        {
            Debug.LogError("Invalid weapon index: " + index);
            return; // Thoát nếu chỉ số không hợp lệ
        }

        // Ẩn súng hiện tại
        weapons[currentWeaponIndex].SetActive(false);

        // Cập nhật chỉ số súng hiện tại
        currentWeaponIndex = index;

        // Hiện súng mới
        weapons[currentWeaponIndex].SetActive(true);

        // Gọi hoạt ảnh đổi súng
        animator.SetTrigger("SwitchWeapon");

        // Cập nhật số đạn khi chuyển súng
        Updateammo();

        // Thêm dòng log để kiểm tra
        Debug.Log("Switched to weapon: " + currentWeaponIndex);
    }

    private void Reload()
    {
        // Gọi hoạt ảnh nạp đạn
        animator.SetTrigger("Reload");
        ammoCounts[currentWeaponIndex] = maxAmmoCounts[currentWeaponIndex]; // Nạp đầy đạn cho súng hiện tại
        Debug.Log("Reloading... Current ammo: " + ammoCounts[currentWeaponIndex]);
        Updateammo(); // Cập nhật số đạn sau khi nạp
    }

    private void Shoot()
    {
        // Kiểm tra xem có đủ đạn để bắn không
        if (ammoCounts[currentWeaponIndex] > 0)
        {
            // Create bullet
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            // Get the Rigidbody component of the bullet
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                // Add force to the bullet
                bulletRb.AddForce(bulletSpawnPoint.forward * 20f, ForceMode.Impulse); // Adjust force as needed
            }

            // You'll need to handle the bullet's collision and damage in a separate script attached to the bullet prefab.
            // For now, let's assume the bullet will eventually hit something.
            // To apply damage directly from here, you would need to use raycasting or spherecasting.
            // However, it's generally better to let the bullet handle its own collision.

            // Decrease ammo
            ammoCounts[currentWeaponIndex]--;
            Debug.Log("Shooting bullet... Remaining ammo: " + ammoCounts[currentWeaponIndex]);
            Updateammo(); // Cập nhật số đạn sau khi bắn
        }
        else
        {
            Debug.Log("Out of ammo! Reload to continue.");
        }
    }

    private void Updateammo()
    {
        if (ammo != null)
        {
            // Cập nhật số đạn hiện tại cho súng đang được chọn
            ammo.text = $"{ammoCounts[currentWeaponIndex]} / {maxAmmoCounts[currentWeaponIndex]}";
        }
    }
}
