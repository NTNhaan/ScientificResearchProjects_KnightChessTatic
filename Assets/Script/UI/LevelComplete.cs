using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelComplete : MonoBehaviour
{
    public void OnLevelComplete(int startAquired)
    {
        if (LevelMenuManager.currLevel == LevelMenuManager.UnlockedLevel && startAquired > 0)
        {
            LevelMenuManager.UnlockedLevel++;
            PlayerPrefs.SetInt("UnlockedLevel", LevelMenuManager.UnlockedLevel);
        }
        if (startAquired > PlayerPrefs.GetInt("starts" + LevelMenuManager.currLevel.ToString(), 0))
            PlayerPrefs.SetInt("Level" + LevelMenuManager.currLevel.ToString(), startAquired);
        SceneManager.LoadScene("Menu");
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
