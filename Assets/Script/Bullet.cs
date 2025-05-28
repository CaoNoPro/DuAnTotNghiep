using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Target"))
        {
            print("hit" + collision.gameObject.name + "!");
            Destroy(gameObject); // Destroy the target object
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            print("hit the wall");
            Destroy(gameObject); // Destroy the target object
        }
    }
}
