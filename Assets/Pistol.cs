using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Pistol : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float ImpactForce = 30f;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f; 
    private bool isReloading = false;


    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject ImpactEffect;

    private float nextTimeToFire = 0f;

    public Animator animator;

    [Header("UI Ammo Display")]
    public TextMeshProUGUI ammoText; 
  

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void OnEnable()
    {
        isReloading = false;
        if (animator != null)
        {
            animator.SetBool("Reloading", false);
        }
        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
            return;
        }


        if (isReloading)
            return; 


        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload()); 
            return; 
        }


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
        UpdateAmmoUI(); 

        if (animator != null)
        {
            animator.SetBool("Reloading", true);
        }

        yield return new WaitForSeconds(reloadTime); 

        currentAmmo = maxAmmo; 
        isReloading = false;

        if (animator != null)
        {
            animator.SetBool("Reloading", false);
        }

        UpdateAmmoUI();
    }

    public void Shoot()
    {

        if (currentAmmo <= 0 || isReloading)
            return;

        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name); 

            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            if (hit.rigidbody != null)
            {

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
                ammoText.text = "Reloading..."; 
            }
            else
            {
                ammoText.text = currentAmmo + "/" + maxAmmo; 
            }
        }
    }
}