using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager i;

    bool triggerLevelLoad = true;
    LevelHash levelHashToLoad;

    void Awake()
    {
        i = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += ChangedActiveScene;

    }

    public void LoadLevel(LevelHash lvl)
    {
        triggerLevelLoad = true;
        levelHashToLoad = lvl;
        SceneManager.LoadScene("MapWorker");
    }
    void ChangedActiveScene(Scene current, Scene next)
    {
        if (next.name == "MapWorker")
        {
            if (triggerLevelLoad)
            {
                triggerLevelLoad = false;
                if (Gridloader.i)
                {
                    Gridloader.i.LoadLevel(levelHashToLoad);
                }
                else
                {
                    Debug.LogError("Map Load Failed:Failed to find gridloader");
                }
            }
        }
    }

    void Update()
    {

    }
}
