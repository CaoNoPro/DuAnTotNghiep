using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public int thirst;
    public int Hunger;
    public float[] position;

    public PlayerData(Player player)
    {
        health = player.health;
        thirst = player.thirst;
        Hunger = player.Hunger;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
