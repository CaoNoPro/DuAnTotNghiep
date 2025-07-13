using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Import thư viện UI để sử dụng Text

public class Pistol : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float ImpactForce = 30f;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f; // Đã đổi tên từ realoadTime cho đúng chính tả
    private bool isReloading = false;


    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject ImpactEffect;

    private float nextTimeToFire = 0f;

    public Animator animator;

    // --- Thêm vào cho UI Ammo Display ---
    [Header("UI Ammo Display")]
    public Text ammoText; // Kéo Text UI vào đây trong Inspector
    // --- End UI Ammo Display ---

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI(); // Cập nhật UI ngay khi bắt đầu
    }

    void OnEnable()
    {
        // OnEnable được gọi khi GameObject hoặc script được bật.
        // Đây là nơi tốt để reset trạng thái nạp đạn và cập nhật UI khi súng được chọn/hiển thị.
        isReloading = false;
        if (animator != null)
        {
            animator.SetBool("Reloading", false);
        }
        UpdateAmmoUI(); // Cập nhật UI khi súng được kích hoạt lại
    }

    void Update()
    {
        // --- Logic Nạp đạn bằng nút R ---
        // 1. Kiểm tra nếu người chơi nhấn phím R
        // 2. Đảm bảo chưa đầy đạn (currentAmmo < maxAmmo)
        // 3. Đảm bảo không đang trong quá trình nạp đạn (isReloading == false)
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload()); // Bắt đầu Coroutine nạp đạn
            return; // Thoát khỏi Update để không xử lý bắn hoặc nạp đạn tự động ngay lập tức
        }
        // --- Kết thúc Logic Nạp đạn bằng nút R ---


        if (isReloading)
            return; // Nếu đang nạp đạn, không làm gì khác (không bắn, không nạp tự động)

        // Tự động nạp đạn khi hết đạn và không đang nạp
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload()); // Bắt đầu coroutine Reload
            return; // Thoát khỏi Update để không xử lý bắn
        }

        // Logic bắn súng
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        UpdateAmmoUI(); // Cập nhật UI để hiển thị trạng thái đang nạp đạn (ví dụ: "Reloading...")

        if (animator != null)
        {
            animator.SetBool("Reloading", true);
        }

        yield return new WaitForSeconds(reloadTime); // Chờ toàn bộ thời gian nạp đạn

        currentAmmo = maxAmmo; // Gán lại đầy đạn
        isReloading = false;

        if (animator != null)
        {
            animator.SetBool("Reloading", false);
        }

        UpdateAmmoUI(); // Cập nhật UI sau khi nạp đạn xong
    }

    public void Shoot()
    {
        // Đảm bảo không bắn khi hết đạn hoặc đang nạp đạn
        if (currentAmmo <= 0 || isReloading)
            return;

        currentAmmo--;
        UpdateAmmoUI(); // Cập nhật UI ngay sau khi bắn

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name); // Thêm debug log để kiểm tra

            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            if (hit.rigidbody != null)
            {
                // Đẩy lùi vật thể ra xa điểm va chạm
                hit.rigidbody.AddForce(-hit.normal * ImpactForce);
            }

            if (ImpactEffect != null)
            {
                GameObject ImpactGo = Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGo, 2f);
            }
        }
    }

    // Hàm để cập nhật UI hiển thị đạn
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            if (isReloading)
            {
                ammoText.text = "Reloading..."; // Hiển thị "Reloading..." khi đang nạp đạn
            }
            else
            {
                ammoText.text = currentAmmo + "/" + maxAmmo; // Ví dụ: "10/10"
            }
        }
    }
}