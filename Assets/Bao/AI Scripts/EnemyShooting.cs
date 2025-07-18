using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public Transform firePoint; // Điểm xuất phát của viên đạn (thường là đầu súng của AI)
    public GameObject bulletPrefab; // Prefab của viên đạn
    public float fireRate = 1f; // Tốc độ bắn (viên đạn/giây)
    public float bulletSpeed = 20f; // Tốc độ của viên đạn

    private Transform currentTarget; // Mục tiêu hiện tại để bắn
    private float nextFireTime;

    void Start()
    {
        if (firePoint == null)
        {
            // Tạo FirePoint nếu chưa có
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0, 1.5f, 0.5f); // Điều chỉnh vị trí này cho phù hợp với model AI
            firePoint = fp.transform;
        }
    }

    void Update()
    {
        if (currentTarget != null && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    void Shoot()
    {
        // Ngắm mục tiêu trước khi bắn (chỉ xoay trục Y)
        Vector3 lookAtTarget = currentTarget.position;
        lookAtTarget.y = transform.position.y;
        transform.LookAt(lookAtTarget);

        // Tạo viên đạn
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            // Tính toán hướng bắn chính xác hơn về phía mục tiêu
            Vector3 direction = (currentTarget.position - firePoint.position).normalized;
            bulletRb.linearVelocity = direction * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet Prefab is missing Rigidbody component!");
        }

        // Tùy chọn: Hủy viên đạn sau một thời gian để không làm đầy scene
        Destroy(bullet, 5f);
    }
}