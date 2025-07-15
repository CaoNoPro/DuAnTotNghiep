using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] playerStats; //[0] - Health, [1] - thirst, [2] - Hunger

    //public string[] inventoryContent;

    public PlayerData(float[] _playerStats)
    {
        playerStats = _playerStats;
    }
}
