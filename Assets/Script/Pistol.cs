using UnityEngine;
public class Pistol : Gun
{
    public override void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        Destroy(bullet,2f);

        muzzleFlash?.Play();
        gunshotSound?.Play();

        currentAmmo--;
    }
}
