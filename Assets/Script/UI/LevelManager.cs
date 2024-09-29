using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levels;
    void Start()
    {
        levels[LevelMenuManager.currLevel].SetActive(true);
    }
}
