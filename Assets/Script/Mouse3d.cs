using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse3d : MonoBehaviour
{
    public static Mouse3d Instance { get; private set; }

    [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            transform.position = raycastHit.point;
        }
    }
    public static Vector3 GetMouseWorldPosition()
    {
        if(Instance == null)
        {
            Debug.LogError("Mouse3d instance is not initialized.");
        }
        return Instance.GetMouseWorldPosition_Instance();
    }
    private Vector3 GetMouseWorldPosition_Instance()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero; // or some default value
        }
    }
}

