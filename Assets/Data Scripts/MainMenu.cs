using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button LoadGameBTN;
    private void Start()
    {
        LoadGameBTN.onClick.AddListener(() =>
        {
            SaveManager.Instance.StartLoadedGame();
        }
        );
    }
    public void NewGame()
    {
        SceneManager.LoadScene("LAB snow");
    }

    public void ExitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
    }
}
