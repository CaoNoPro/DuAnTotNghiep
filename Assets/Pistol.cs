using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float FireRate = 15f;
    public float impactForce = 30f;
    private bool isReload = false;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextTimeToFire = 0f;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReload)
            return;

        if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / FireRate;
                shoot();
            }
    }

    IEnumerator Reload()
    {
        isReload = false;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds (reloadTime);

        currentAmmo = maxAmmo;
    }

    void shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
    } 
}
