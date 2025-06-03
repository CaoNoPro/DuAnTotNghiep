using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float ShootingDelay = 2f;

    public int bulletPerBurst = 3; // Number of bullets to shoot in a burst
    public int BurstBulletLeft;

    public float SpeardIntensity;

    //bullet
    public GameObject bulletPrefab; // Prefab of the bullet to be instantiated
    public Transform firePoint; // Point from where the bullet will be fired
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f; // Time after which the bullet will be destroyed

    public GameObject muzzleEffect; // Muzzle effect to be instantiated when shooting
    private Animator animator;

    //Loading
    public float reloadTime;
    public float magazineSize, bulletLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    public enum WeaponModel
    {
        Pistol1911,
        M4A1
    }
    
    public WeaponModel thisWeaponModel; // Current weapon model being used

    public enum ShootingMode
    {
        Single, // Single shot mode
        Burst, // Burst shot mode
        Auto // Automatic shooting mode
    }

    public ShootingMode shootingMode; // Current shooting mode

    private void Awake()
    {
        readyToShoot = true;
        BurstBulletLeft = bulletPerBurst; // Initialize the current burst count to the number of bullets per burst
        animator = GetComponent<Animator>(); // Get the Animator component attached to the weapon
        bulletLeft = magazineSize; // Initialize the number of bullets left in the magazine to the magazine size 
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveWeapon)
        {
            if (bulletLeft == 0 && isShooting)
            {
                SoundManager.Instance.EmptyMag1911.Play(); // Play empty magazine sound if no bullets left and shooting is attempted

            }
            if (shootingMode == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0); // Check if the left mouse button is pressed for automatic shooting
            }
            else if (shootingMode == ShootingMode.Burst || shootingMode == ShootingMode.Single)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0); // Check if the left mouse button is pressed for burst shooting
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletLeft < magazineSize && isReloading == false) // Check if the R key is pressed to reload
            {
                Reload(); // Call the Reload method
            }

            if (readyToShoot && isShooting && bulletLeft <= 0 && isReloading == false) // Check if ready to shoot and no bullets left
            {
                Reload(); // Call the Reload method
            }

            if (readyToShoot && isShooting && bulletLeft > 0)
            {
                BurstBulletLeft = bulletPerBurst;
                Shoot();
            }

            if (AmmoManager.Instance.ammoDisplay != null) // Check if no bullets left and not reloading
            {
                AmmoManager.Instance.ammoDisplay.text = $"{bulletLeft / bulletPerBurst}/{magazineSize / bulletPerBurst}"; // Update the ammo display text with the number of bullets left
            }
        }
    }

    private void Shoot()
    {
        bulletLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play(); // Play the muzzle effect particle system
        animator.SetTrigger("Recoil"); // Trigger the shooting animation

        //SoundManager.Instance.shootingSound1911.Play(); // Play the shooting sound effect

        SoundManager.Instance.playShootingSound(thisWeaponModel); // Play the shooting sound based on the weapon model

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpeard().normalized;


        GameObject bullet =Instantiate(bulletPrefab,firePoint.position,Quaternion.identity); // Instantiate the bullet at the fire point

        bullet.transform.forward = shootingDirection; // Set the bullet's forward direction to the calculated shooting direction

        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward.normalized * bulletVelocity, ForceMode.Impulse); // Set the bullet's velocity


        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime)); // Start the coroutine to destroy the bullet after a certain time

        if(allowReset)
        {
            Invoke("ResetShot", ShootingDelay); // Reset the shooting state after a delay
            allowReset = false;
        }

        if(shootingMode == ShootingMode.Burst && BurstBulletLeft > 1)
        {
            BurstBulletLeft--;
            Invoke("Shoot", ShootingDelay);
        }
    }

    private void Reload()
    {
        SoundManager.Instance.playReloadSound(thisWeaponModel); // Play the reload sound based on the weapon model

        animator.SetTrigger("Reload"); // Trigger the reloading animation 
        isReloading = true; // Set the reloading state to true
        Invoke("ReloadComplete", reloadTime); // Start the reloading process after a delay
    }

    private void ResetShot()
    {
        readyToShoot = true; // Reset the shooting state to allow shooting again
        allowReset = true; // Allow resetting the shot again
    }

    private void ReloadComplete()
    {
        SoundManager.Instance.ReloadMag1911.Play();
        bulletLeft = magazineSize;
        isReloading = false;

    }

    private Vector3 CalculateDirectionAndSpeard()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Create a ray from the center of the camera viewport
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit)) // Check if the ray hits any object
        {
            targetPoint = hit.point; // If it hits, use the hit point as the target point
        }
        else
        {
            targetPoint = ray.GetPoint(100); // If it doesn't hit anything, use a point far away in the ray's direction
        }

        Vector3 direction = targetPoint - firePoint.position; // Calculate the direction from the fire point to the target point

        float x = UnityEngine.Random.Range(-SpeardIntensity, SpeardIntensity); // Randomly adjust the x component for spread
        float y = UnityEngine.Random.Range(-SpeardIntensity, SpeardIntensity); // Randomly adjust the y component for spread

        return direction + new Vector3(x, y, 0); // Return the direction with the spread adjustments
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPrefabLifeTime)
    {
        yield return new WaitForSeconds(bulletPrefabLifeTime); // Wait for the specified time
        Destroy(bullet); // Destroy the bullet
    }
}
