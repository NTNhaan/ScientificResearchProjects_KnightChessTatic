using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public GameObject PlayMenu, LobbyGame, LevelMenu, CreateMenu;
    public void OnClickPlay()
    {
        PlayMenu.SetActive(true);
    }
    public void OnClickOnline()
    {
        LobbyGame.SetActive(true);
    }
    public void OnClickMap()
    {
        LevelMenu.SetActive(true);
    }
    public void OnClickBot()
    {
        //SceneManager.LoadScene("Gameplay");
    }
    public void OnClickCreateRoom()
    {
        CreateMenu.SetActive(true);
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
}
