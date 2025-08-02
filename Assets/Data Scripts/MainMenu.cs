using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button LoadGameBTN;
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
