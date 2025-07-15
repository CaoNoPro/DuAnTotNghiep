using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public bool isSavingToJson;

    #region || ------General Section------||

    public void SaveGame()
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();

        SaveAllGameData(data);
    }

    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        playerStats[0] = PlayerState.Instance.currentHealth;
        playerStats[1] = PlayerState.Instance.currentThirst;
        playerStats[2] = PlayerState.Instance.currentHunger;
        
        return new PlayerData(playerStats);
    }

    public void SaveAllGameData(AllGameData gameData)
    {
        if (isSavingToJson)
        {
            //saveGameDataToJsonFile(gameData);

        }
        else
        {
            SaveGameToBinaryFile(gameData);
        }
    }


    #endregion

    #region || ------To Binary Section------||

    public void SaveGameToBinaryFile(AllGameData gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/save_game.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data saved to" + Application.persistentDataPath + "/Save_game.bin");

    }

    public AllGameData LoadGameToBinaryFile()
    {
        string path = Application.persistentDataPath + "/Save_game.bin";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            return data;
        }
        return null;
    }


    #endregion
    #region || ------Settings Section------||
    #region || ------Volume Settings------||
    [System.Serializable]
    public class VolumeSetting
    {
        public float music;
        public float effects;
        public float master;
    }
    public void SaveVolumeSetting(float _music, float _effects, float _master)
    {
        VolumeSetting volumeSetting = new VolumeSetting()
        {
            music = _music,
            effects = _effects,
            master = _master
        };
        PlayerPrefs.SetString("volume", JsonUtility.ToJson(volumeSetting));
        PlayerPrefs.Save();

    }

    public VolumeSetting LoadVolumeSetting()
    {
        return JsonUtility.FromJson<VolumeSetting>(PlayerPrefs.GetString("volume"));
    }
    public float LoadMusic()
    {
        var volumeSetting = JsonUtility.FromJson<VolumeSetting>(PlayerPrefs.GetString("volume"));
        return volumeSetting.music;
    }
    #endregion

    #endregion
}

