using UnityEngine;

public class WeaponSystemManager : MonoBehaviour
{
    [Header("References")]
    public Animator animator; // Animator của nhân vật
    public int currentWeaponIndex = 0; // Chỉ số súng hiện tại
    public GameObject[] weapons; // Danh sách các súng
    public GameObject bulletPrefab; // Prefab của đạn
    public Transform bulletSpawnPoint; // Điểm xuất phát của đạn

    private void Start()
    {
        // Ẩn tất cả các súng ngoại trừ súng đầu tiên
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == currentWeaponIndex);
        }
    }

    private void Update()
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
        if (index < 0 || index >= weapons.Length) return;

        // Ẩn súng hiện tại
        weapons[currentWeaponIndex].SetActive(false);
        
        // Cập nhật chỉ số súng hiện tại
        currentWeaponIndex = index;

        // Hiện súng mới
        weapons[currentWeaponIndex].SetActive(true);

        // Gọi hoạt ảnh đổi súng
        animator.SetTrigger("SwitchWeapon");
    }

    private void Reload()
    {
        // Gọi hoạt ảnh nạp đạn
        animator.SetTrigger("Reload");
    }

    private void Shoot()
    {
        // Tạo đạn
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        
        // Thêm lực cho đạn
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(bulletSpawnPoint.forward * 20f, ForceMode.Impulse); // Điều chỉnh lực theo nhu cầu
    }
}
