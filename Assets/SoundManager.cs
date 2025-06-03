using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChannel;

    public AudioClip Pistol1911Shoot;
    public AudioClip M4A1Shoot;

    public AudioSource ReloadMag1911;
    public AudioSource ReloadMagM4A1;

    public AudioSource EmptyMag1911;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
        else
        {
            Instance = this;
        }
    }

    public void playShootingSound(WeaponModel weapon)
    {
        switch(weapon)
        {
            case WeaponModel.Pistol1911:
                if (ShootingChannel != null)
                {
                    ShootingChannel.PlayOneShot(Pistol1911Shoot);
                }
                break;
            case WeaponModel.M4A1:
                ShootingChannel.PlayOneShot(M4A1Shoot);
                break;
        }
    }
    public void playReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                if (ReloadMag1911 != null)
                {
                    ReloadMag1911.Play();
                }
                break;
            case WeaponModel.M4A1:
                ReloadMagM4A1.Play();
                break;
        }
    }
    }
