[System.Serializable]
public class PlayerData
{
    public float[] playerStats; //[0] - Health, [1] - thirst, [2] - Hunger
    public float[] playerPositionAndRotation; // vi tri dung voi chuot
    public string[] inventoryContent; // luu vat pham trong balo
    //public string[] quickSlotContent;

    public PlayerData(float[] _playerStats, float[] _playerPosAndRot, string[] _inventoryContent ) //string[] _quickSlotContent
    {
        playerStats = _playerStats;
        playerPositionAndRotation = _playerPosAndRot;
        inventoryContent = _inventoryContent;
        //quickSlotContent = _quickSlotContent;
    }
}
