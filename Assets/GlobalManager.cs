using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager i;

    void Awake()
    {
        i = this;
        DontDestroyOnLoad(gameObject);

    }

    public void LoadLevel(LevelHash lvl){
        
    }

    void Update()
    {
        
    }
}
