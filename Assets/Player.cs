using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;
    public int thirst = 100;
    public int Hunger = 100;

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        health = data.health;
        thirst = data.thirst;
        Hunger = data.Hunger;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;
    }
}
