using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public GameObject quit;
    public GameObject Setting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quit.SetActive(false);
        Setting.SetActive(false);
    }

    // Update is called once per frame

    public void LoadScene()
    {
        SceneManager.LoadScene("LAB snow");
        Debug.Log("Scene loaded: LAB snow");
    }

    public void SettingGame()
    {
        Setting.SetActive(true);
    }
    public void QuitGame()
    {
        quit.SetActive(true);
    }
    public void ConfirmQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void NotSureQuit()
    {
        quit.SetActive(false);
    }
}
