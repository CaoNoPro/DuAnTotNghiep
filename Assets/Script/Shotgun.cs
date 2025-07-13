using UnityEngine;

public class Shotgun : Gun
{
    public int pelletCount = 8; // Number of pellets fired per shot
    public float spreadAngle = 15f; // Spread angle for the pellets

    public override void Shoot()
    {
        for(int i = 0; i < pelletCount; i++)
        {
            Quaternion spread = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0));
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, spread);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletForce, ForceMode.Impulse);
            Destroy(bullet, 2f);
        }
        muzzleFlash?.Play();
        gunshotSound?.Play();

        currentAmmo--;
    }
}
