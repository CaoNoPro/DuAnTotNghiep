using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] playerStats; //[0] - Health, [1] - thirst, [2] - Hunger
    public float[] playerPositionAndRotation;

    //public string[] inventoryContent;

    public PlayerData(float[] _playerStats, float[] _playerPosAndRot)
    {
        playerStats = _playerStats;
        playerPositionAndRotation = _playerPosAndRot;
    }
}
