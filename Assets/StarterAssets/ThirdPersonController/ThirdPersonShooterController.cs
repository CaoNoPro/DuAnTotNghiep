using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera aimCamera; // Updated to use CinemachineCamera
    [SerializeField] private float normalSensitivity ;
    [SerializeField] private float aimSensitivity ;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform bulletpref;
    [SerializeField] private Transform spawnBullet;


    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }
        if (starterAssetsInputs.aim)
        {
            aimCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false); // Disable rotation on movement while aiming

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y; // Keep the aim target at the same height as the player
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f); // Smoothly rotate towards the aim direction
        }
        else
        {
            aimCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true); // Enable rotation on movement when not aiming
        }

        if (starterAssetsInputs.shoot)
        {
            Vector3 aimDirection = (mouseWorldPosition - spawnBullet.position).normalized;
            Instantiate(bulletpref, spawnBullet.position,Quaternion.LookRotation(aimDirection, Vector3.up));
            starterAssetsInputs.shoot = false; // Reset shoot input after shooting
        }
    }
}
