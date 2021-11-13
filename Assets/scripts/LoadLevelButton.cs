using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevelButton : MonoBehaviour
{
    [SerializeField]
    LevelHash level;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(clicked);
    }

    void clicked(){
        GlobalManager.i.LoadLevel(level);
    }
}
