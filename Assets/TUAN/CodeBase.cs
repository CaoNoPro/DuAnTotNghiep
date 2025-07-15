using UnityEngine;

public class CodeBasedMovement : MonoBehaviour
{
    public float speed = 5f;
    // Khoảng cách của "cây gậy dò đường", nên lớn hơn 0 một chút
    public float collisionCheckDistance = 0.5f; 

    void Update()
    {
        // 1. Lấy hướng di chuyển từ input của người chơi
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Nếu người chơi đang có ý định di chuyển
        if (moveDirection.magnitude > 0.1f)
        {
            // 2. Kiểm tra va chạm TRƯỚC KHI di chuyển
            if (CanMove(moveDirection))
            {
                // 3. Nếu không có vật cản, thực hiện di chuyển
                transform.Translate(moveDirection * speed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Kiểm tra xem có thể di chuyển theo hướng được chỉ định hay không.
    /// </summary>
    /// <param name="direction">Hướng di chuyển.</param>
    /// <returns>True nếu có thể di chuyển, False nếu có vật cản.</returns>
    private bool CanMove(Vector3 direction)
    {
        // Bắn một tia Raycast từ vị trí của người chơi, theo hướng di chuyển,
        // với khoảng cách bằng collisionCheckDistance.
        // `!Physics.Raycast(...)` có nghĩa là "Nếu tia Raycast KHÔNG trúng bất cứ thứ gì"
        
        RaycastHit hit; // Biến để lưu thông tin vật thể bị bắn trúng
        if (Physics.Raycast(transform.position, direction, out hit, collisionCheckDistance))
        {
            // Tia đã trúng một vật gì đó. Log ra để debug.
            Debug.Log("Không thể di chuyển, vướng phải: " + hit.collider.name);
            return false; // Trả về false: không được phép di chuyển
        }

        // Nếu không trúng gì cả, tức là đường đi trống
        return true; // Trả về true: được phép di chuyển
    }
    
    // (Optional) Vẽ tia Raycast trong Scene view để dễ hình dung
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Gizmos.DrawRay(transform.position, moveDirection * collisionCheckDistance);
    }
}