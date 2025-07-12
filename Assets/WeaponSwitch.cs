using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public int WeaponSelect = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponSelect();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = WeaponSelect;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (WeaponSelect >= transform.childCount - 1)
                WeaponSelect = 0;
            else
                WeaponSelect++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (WeaponSelect <= 0)
                WeaponSelect = transform.childCount - 1;
            else
                WeaponSelect--;
        }

        if (previousSelectedWeapon != WeaponSelect)
        {
            weaponSelect();
        }
    }
    void weaponSelect()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == WeaponSelect)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
