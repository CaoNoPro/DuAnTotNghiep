using UnityEngine;
using System.Collections;

public abstract class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate;
    public int magazineSize;
    public float bulletForce;
    public float reloadTime;
    public ParticleSystem muzzleFlash;
    public AudioSource gunshotSound;

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float nextFireTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        currentAmmo = magazineSize;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isReloading)
            return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }
    public abstract void Shoot();
    protected IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
    }
}
