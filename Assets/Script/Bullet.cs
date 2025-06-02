using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Target"))
        {
            print("hit" + collision.gameObject.name + "!");
            CreateBulletImpactEffect(collision); // Create bullet impact effect
            Destroy(gameObject); // Destroy the target object
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            print("hit the wall");
            CreateBulletImpactEffect(collision); // Create bullet impact effect
            Destroy(gameObject); // Destroy the target object
        }
    }
    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0]; // Get the contact point of the collision

        GameObject hole = Instantiate(
            GlobalRefences.Instance.bulletImpactEffectPrefab, // Use the bullet impact effect prefab from GlobalRefences
            contact.point, // Set the position to the contact point of the collision
            Quaternion.LookRotation(contact.normal) // Set the rotation to the normal of the contact point
            );

        hole.transform.SetParent(objectWeHit.gameObject.transform); // Set the parent of the hole to the object we hit


    }
}
