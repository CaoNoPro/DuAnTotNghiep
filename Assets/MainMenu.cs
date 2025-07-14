using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
