using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelMenuManager : MonoBehaviour
{
    public LevelObject[] levelObjects;
    public Sprite[] GoldenStartSprite;
    public static int currLevel;
    public static int UnlockedLevel;
    public void OnClickLevel(int levelNum)
    {
        Debug.Log("Level " + levelNum);
        currLevel = levelNum;
        SceneManager.LoadScene("LevelScene");
    }
    public void Start()
    {
        UnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
        for (int i = 0; i < levelObjects.Length; i++)
        {
            if (UnlockedLevel >= i)
            {
                levelObjects[i].levelButton.interactable = true;
                int starts = PlayerPrefs.GetInt("Level" + i.ToString(), 0);
                levelObjects[i].starts.sprite = GoldenStartSprite[starts];
            }
        }
    }
}
