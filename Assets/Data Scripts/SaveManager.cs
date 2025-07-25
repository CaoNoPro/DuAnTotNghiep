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

    //Json Project Save Path
    string JsonProjectPath;
    //Json External/Real Save Path
    string JsonPathPersistantl;
    //Binary Save Path
    string binarypath;

    public bool isSavingToJson;

    private void Start()
    {
        JsonProjectPath = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveGame.json";
        JsonPathPersistantl = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveGame.json";
        binarypath = Application.persistentDataPath + "/save_game.bin";
    }

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
            SaveGameToJsonFile(gameData);

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
            AllGameData gameData = LoadGameToJsonFile();
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

        FileStream stream = new FileStream(binarypath, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data saved to" + binarypath);

    }

    public AllGameData LoadGameToBinaryFile()
    {

        if (File.Exists(binarypath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binarypath, FileMode.Open);
            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Data Loaded from" + binarypath);

            return data;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region || ------To Json Section------||

    public void SaveGameToJsonFile(AllGameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);

        string encryption = EncryptionDecryption(json);

        using (StreamWriter writer = new StreamWriter(JsonProjectPath))
        {
            writer.Write(encryption);
            print("Saved game to Json File at " + JsonProjectPath);
        }
        ;
    }

    public AllGameData LoadGameToJsonFile()
    {
        using (StreamReader reader = new StreamReader(JsonProjectPath))
        {
            string json = reader.ReadToEnd();

            string decypted = EncryptionDecryption(json);

            AllGameData data = JsonUtility.FromJson<AllGameData>(decypted);
            return data;
        }
        ;
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

    #region || ------Encryption------||

    public string EncryptionDecryption(string data)
    {
        string Keyword = "1234567";
        string result = "";

        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ Keyword[i % Keyword.Length]);
        }
        return result;


    }





    #endregion
}

