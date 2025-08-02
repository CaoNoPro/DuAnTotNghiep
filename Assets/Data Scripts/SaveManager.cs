using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Newtonsoft.Json; // Thêm dòng này để sử dụng Newtonsoft.Json

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; } // Thay đổi thành private set cho kiểm soát tốt hơn

    // Dữ liệu tạm thời để giữ giữa các lần tải scene
    private AllGameData _pendingGameData;
    private int _pendingSlotNumber = -1; // Khởi tạo với giá trị không hợp lệ

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo SaveManager tồn tại giữa các scene
        }
    }

    //Json Project Save Path (Sẽ chuyển sang PersistentDataPath để nhất quán)
    string JsonProjectPath; // Vẫn khai báo nhưng sẽ không dùng cho file save chính thức
    //Json External/Real Save Path (Được sử dụng cho các tệp JSON)
    string JsonPathPersistent; // Đã đổi tên để rõ ràng hơn
    //Binary Save Path
    string binarypath;

    public bool isSavingToJson; // Biến cờ chọn loại lưu

    string fileName = "SaveGame"; // Tên tệp save

    public bool isLoading; // Trạng thái đang tải

    private void Start()
    {
        // JsonProjectPath = Application.dataPath + Path.AltDirectorySeparatorChar; // Không nên lưu game vào Application.dataPath
        JsonPathPersistent = Application.persistentDataPath + Path.AltDirectorySeparatorChar; // Đường dẫn lưu trữ dữ liệu người dùng
        binarypath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;

        // Đăng ký sự kiện khi một scene mới được tải
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Phương thức này được gọi mỗi khi một scene được tải
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Nếu đang trong quá trình tải game và có dữ liệu chờ
        if (isLoading && _pendingGameData != null)
        {
            Debug.Log($"Applying loaded data to scene: {scene.name}");
            ApplyLoadedGameData(_pendingGameData); // Áp dụng dữ liệu đã tải

            // Đặt lại trạng thái và dữ liệu chờ
            isLoading = false;
            _pendingGameData = null;
            _pendingSlotNumber = -1;
        }
    }

    #region || ------General Section------||

    #region || ------Saving------||
    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();
        data.enviromenData = GetEnviromenData();

        SelectSaveType(data, slotNumber);
    }

    private EnviromenData GetEnviromenData()
    {
        // Kiểm tra InventorySystem.Instance không null trước khi truy cập
        if (InventorySystem.Instance != null)
        {
            List<string> itemsPickedUp = InventorySystem.Instance.itemPickedUp;
            return new EnviromenData(itemsPickedUp);
        }
        else
        {
            Debug.LogWarning("InventorySystem.Instance is null when trying to get environment data for saving. Returning empty data.");
            return new EnviromenData(new List<string>()); // Trả về dữ liệu rỗng nếu không tìm thấy InventorySystem
        }
    }

    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        // Đảm bảo PlayerState.Instance không null trước khi truy cập thuộc tính
        if (PlayerState.Instance != null)
        {
            playerStats[0] = PlayerState.Instance.currentHealth;
            playerStats[1] = PlayerState.Instance.currentThirst;
            playerStats[2] = PlayerState.Instance.currentHunger;
        }
        else
        {
            Debug.LogWarning("PlayerState.Instance is null when trying to get player stats for saving. Using default values.");
            playerStats[0] = 100f; // Giá trị mặc định
            playerStats[1] = 100f;
            playerStats[2] = 100f;
        }

        float[] playerPositionAndRotation = new float[6];
        if (PlayerState.Instance != null && PlayerState.Instance.playerBody != null)
        {
            playerPositionAndRotation[0] = PlayerState.Instance.playerBody.transform.position.x;
            playerPositionAndRotation[1] = PlayerState.Instance.playerBody.transform.position.y;
            playerPositionAndRotation[2] = PlayerState.Instance.playerBody.transform.position.z;

            // Lưu góc Euler thay vì các thành phần Quaternion raw
            playerPositionAndRotation[3] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.x;
            playerPositionAndRotation[4] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.y;
            playerPositionAndRotation[5] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.z;
        }
        else
        {
            Debug.LogWarning("PlayerState.Instance or playerBody is null when trying to get player position/rotation for saving. Using default values.");
            // Giá trị mặc định nếu không tìm thấy PlayerState/playerBody
            playerPositionAndRotation[0] = 0f; playerPositionAndRotation[1] = 0f; playerPositionAndRotation[2] = 0f;
            playerPositionAndRotation[3] = 0f; playerPositionAndRotation[4] = 0f; playerPositionAndRotation[5] = 0f;
        }

        // Kiểm tra InventorySystem.Instance không null trước khi lấy itemList
        string[] inventory = (InventorySystem.Instance != null) ? InventorySystem.Instance.itemList.ToArray() : new string[0];

        return new PlayerData(playerStats, playerPositionAndRotation, inventory);
    }

    public void SelectSaveType(AllGameData gameData, int slotNumber)
    {
        if (isSavingToJson)
        {
            SaveGameToJsonFile(gameData, slotNumber);
        }
        else
        {
            SaveGameToBinaryFile(gameData, slotNumber);
        }
    }
    #endregion

    #region || ------Loading------||

    public AllGameData LoadingTypeSwitch(int slotNumber)
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameToJsonFile(slotNumber);
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameToBinaryFile(slotNumber);
            return gameData;
        }
    }

    // Phương thức này tải dữ liệu và chuẩn bị cho việc chuyển scene
    public void LoadGameDataForSceneTransition(int slotNumber)
    {
        _pendingGameData = LoadingTypeSwitch(slotNumber); // Tải dữ liệu vào biến tạm
        _pendingSlotNumber = slotNumber;
        isLoading = true; // Đặt trạng thái đang tải
    }

    // Phương thức này áp dụng dữ liệu đã tải vào các hệ thống trong scene hiện tại
    private void ApplyLoadedGameData(AllGameData gameData)
    {
        if (gameData == null)
        {
            Debug.LogError("No game data to apply.");
            return;
        }

        // Áp dụng dữ liệu người chơi
        SetPlayerData(gameData.playerData);

        // Áp dụng dữ liệu môi trường
        SetEnviromenData(gameData.enviromenData);

        Debug.Log("Game data applied to the current scene.");
    }

    private void SetPlayerData(PlayerData playerData)
    {
        if (PlayerState.Instance == null)
        {
            Debug.LogError("PlayerState.Instance is null. Cannot set player data.");
            return;
        }

        // Đặt lại chỉ số người chơi
        PlayerState.Instance.currentHealth = playerData.playerStats[0];
        PlayerState.Instance.currentThirst = playerData.playerStats[1];  // Lỗi: playerStats[1] là float, không thể Add
        PlayerState.Instance.currentHunger = playerData.playerStats[2];

        // Đặt lại vị trí
        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1];
        loadedPosition.z = playerData.playerPositionAndRotation[2];
        PlayerState.Instance.playerBody.transform.position = loadedPosition;

        // Đặt lại góc quay từ Euler Angles
        Vector3 loadedRotationEuler;
        loadedRotationEuler.x = playerData.playerPositionAndRotation[3];
        loadedRotationEuler.y = playerData.playerPositionAndRotation[4];
        loadedRotationEuler.z = playerData.playerPositionAndRotation[5];
        PlayerState.Instance.playerBody.transform.rotation = Quaternion.Euler(loadedRotationEuler);

        // Đặt lại nội dung ba lô
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.itemList.Clear(); // Xóa các vật phẩm hiện có để tránh trùng lặp
            foreach (string item in playerData.inventoryContent)
            {
                InventorySystem.Instance.AddToInventory(item);
            }
        }
        else
        {
            Debug.LogWarning("InventorySystem.Instance is null. Cannot set inventory data.");
        }

        // Các logic cho quickSlot (nếu có) sẽ tương tự
    }

    private void SetEnviromenData(EnviromenData enviromenData)
    {
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem.Instance is null. Cannot set environment data (items picked up).");
            return;
        }

        // Xóa các vật phẩm đã nhặt hiện có và thêm lại
        InventorySystem.Instance.itemPickedUp.Clear(); // Cần đảm bảo InventorySystem có phương thức Clear() hoặc truy cập trực tiếp
        foreach (string item in enviromenData.pickupItems) // Sử dụng 'pickupItems' từ EnviromenData.cs
        {
            InventorySystem.Instance.itemPickedUp.Add(item);
        }
    }


    public void StartLoadedGame(int slotNumber)
    {
        LoadGameDataForSceneTransition(slotNumber); // Tải dữ liệu vào biến tạm trước
        SceneManager.LoadScene("CIty new 2"); // Sau đó tải scene mới. Dữ liệu sẽ được áp dụng trong OnSceneLoaded
    }

    // Coroutine DelayLoading không còn cần thiết với phương pháp SceneManager.sceneLoaded
    // private IEnumerator DelayLoading(int slotNumber)
    // {
    //     yield return new WaitForSeconds(1f);
    //     LoadGame(slotNumber);
    //     print("Data Loaded from" + Application.persistentDataPath + "/Save_game.bin");
    // }

    #endregion

    #endregion

    #region || ------To Binary Section------||

    public void SaveGameToBinaryFile(AllGameData gameData, int slotNumber)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        // Sử dụng Application.persistentDataPath để lưu
        FileStream stream = new FileStream(binarypath + fileName + slotNumber + ".bin", FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data saved to" + binarypath + fileName + slotNumber + ".bin");
    }

    public AllGameData LoadGameToBinaryFile(int slotNumber)
    {
        // Sử dụng Application.persistentDataPath để tải
        if (File.Exists(binarypath + fileName + slotNumber + ".bin"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binarypath + fileName + slotNumber + ".bin", FileMode.Open);
            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Data Loaded from" + binarypath + fileName + slotNumber + ".bin");

            return data;
        }
        else
        {
            Debug.LogWarning("Binary save file not found: " + binarypath + fileName + slotNumber + ".bin");
            return null;
        }
    }

    #endregion

    #region || ------To Json Section------||

    public void SaveGameToJsonFile(AllGameData gameData, int slotNumber)
    {
        // Sử dụng JsonConvert.SerializeObject của Newtonsoft.Json
        string json = JsonConvert.SerializeObject(gameData, Formatting.Indented); // Formatting.Indented để dễ đọc

        string encryption = EncryptionDecryption(json);

        // Sử dụng Application.persistentDataPath để lưu
        string path = JsonPathPersistent + fileName + slotNumber + ".json";
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.Write(encryption);
            print("Saved game to Json File at " + path);
        }
    }

    public AllGameData LoadGameToJsonFile(int slotNumber)
    {
        // Sử dụng Application.persistentDataPath để tải
        string path = JsonPathPersistent + fileName + slotNumber + ".json";

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();

                string decrypted = EncryptionDecryption(json);

                // Sử dụng JsonConvert.DeserializeObject của Newtonsoft.Json
                AllGameData data = JsonConvert.DeserializeObject<AllGameData>(decrypted);
                return data;
            }
        }
        else
        {
            Debug.LogWarning("JSON save file not found: " + path);
            return null;
        }
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
        PlayerPrefs.SetString("volume", JsonConvert.SerializeObject(volumeSetting)); // Sử dụng JsonConvert
        PlayerPrefs.Save();
    }

    public VolumeSetting LoadVolumeSetting()
    {
        if (PlayerPrefs.HasKey("volume"))
        {
            return JsonConvert.DeserializeObject<VolumeSetting>(PlayerPrefs.GetString("volume")); // Sử dụng JsonConvert
        }
        else
        {
            // Trả về cài đặt mặc định nếu không có dữ liệu lưu
            return new VolumeSetting { music = 0.5f, effects = 0.5f, master = 1.0f };
        }
    }
    public float LoadMusic()
    {
        var volumeSetting = LoadVolumeSetting();
        return volumeSetting.music;
    }
    #endregion

    #endregion

    #region || ------Encryption------||
    public string EncryptionDecryption(string data)
    {
        string Keyword = "1234567"; // Cân nhắc một key mạnh hơn cho game thực tế
        string result = "";

        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ Keyword[i % Keyword.Length]);
        }
        return result;
    }
    #endregion

    #region || ------Utility------||

    public bool DoesFileExists(int slotNumber)
    {
        if (isSavingToJson)
        {
            // Kiểm tra trong Application.persistentDataPath cho JSON
            return System.IO.File.Exists(JsonPathPersistent + fileName + slotNumber + ".json");
        }
        else
        {
            return System.IO.File.Exists(binarypath + fileName + slotNumber + ".bin");
        }
    }

    public bool isSlotEmpty(int slotNumber)
    {
        return !DoesFileExists(slotNumber);
    }

    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        if (myEventSystem != null)
        {
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
        else
        {
            Debug.LogWarning("EventSystem not found. Cannot deselect button.");
        }
    }
    #endregion
}