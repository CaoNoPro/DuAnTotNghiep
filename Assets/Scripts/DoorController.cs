using UnityEngine;

public class DoorController : MonoBehaviour
{
    public KeyType requiredKey;
    public Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.HasKey(requiredKey))
            {
                OpenDoor();
            }
            else
            {
                Debug.Log("Missing key: " + requiredKey);
            }
        }
    }

    void OpenDoor()
    {
        Debug.Log("Door opened: " + requiredKey);
        animator.SetTrigger("Open");
    }
}
