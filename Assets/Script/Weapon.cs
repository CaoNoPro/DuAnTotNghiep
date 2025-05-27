using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab of the bullet to be instantiated
    public Transform firePoint; // Point from where the bullet will be fired
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f; // Time after which the bullet will be destroyed

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)) // Check if the left mouse button is pressed
        {
            Shoot(); // Call the Shoot method
        }
    }

    private void Shoot()
    {
        
        GameObject bullet =Instantiate(bulletPrefab,firePoint.position,Quaternion.identity); // Instantiate the bullet at the fire point
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward.normalized * bulletVelocity, ForceMode.Impulse); // Set the bullet's velocity
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime)); // Start the coroutine to destroy the bullet after a certain time
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPrefabLifeTime)
    {
        yield return new WaitForSeconds(bulletPrefabLifeTime); // Wait for the specified time
        Destroy(bullet); // Destroy the bullet
    }
}
