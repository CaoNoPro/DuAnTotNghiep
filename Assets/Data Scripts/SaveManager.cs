using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

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

    #region || ------Saving------||
    public void SaveGame()
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();

        SelectSaveType(data);
    }

    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        playerStats[0] = PlayerState.Instance.currentHealth;
        playerStats[1] = PlayerState.Instance.currentThirst;
        playerStats[2] = PlayerState.Instance.currentHunger;

        float[] playerPositionAndRotation = new float[6];
        playerPositionAndRotation[0] = PlayerState.Instance.playerBody.transform.position.x;
        playerPositionAndRotation[1] = PlayerState.Instance.playerBody.transform.position.y;
        playerPositionAndRotation[2] = PlayerState.Instance.playerBody.transform.position.z;

        playerPositionAndRotation[3] = PlayerState.Instance.playerBody.transform.rotation.x;
        playerPositionAndRotation[4] = PlayerState.Instance.playerBody.transform.rotation.y;
        playerPositionAndRotation[5] = PlayerState.Instance.playerBody.transform.rotation.z;
        
        return new PlayerData(playerStats, playerPositionAndRotation);
    }

    public void SelectSaveType(AllGameData gameData)
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

    #region || ------Loading------||

    public AllGameData LoadingTypeSwitch()
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameToBinaryFile();
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameToBinaryFile();
            return gameData;
        }
    }

    public void LoadGame()
    {
        //player Data
        SetPlayerData(LoadingTypeSwitch().playerData);
        //Enviroment Data

    }

    private void SetPlayerData(PlayerData playerData)
    {
        //Setting player stats
        PlayerState.Instance.currentHealth = playerData.playerStats[0];
        PlayerState.Instance.currentThirst = playerData.playerStats[1];
        PlayerState.Instance.currentHunger = playerData.playerStats[2];

        //setting position
        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1];
        loadedPosition.z = playerData.playerPositionAndRotation[2];

        PlayerState.Instance.playerBody.transform.position = loadedPosition;

        Vector3 loadedRotation;
        loadedRotation.x = playerData.playerPositionAndRotation[3];
        loadedRotation.y = playerData.playerPositionAndRotation[4];
        loadedRotation.z = playerData.playerPositionAndRotation[5];

        PlayerState.Instance.playerBody.transform.rotation = Quaternion.Euler(loadedRotation);

    }

    public void StartLoadedGame()
    {
        SceneManager.LoadScene("CIty new 2");
        StartCoroutine(DelayLoading());
    }

    private IEnumerator DelayLoading()
    {
        yield return new WaitForSeconds(1f);

        LoadGame();

        print("Data Loaded from" + Application.persistentDataPath + "/Save_game.bin");
    }


    #endregion

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

