using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class debug_item : MonoBehaviour
{
    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "LevelMaker")
            gameObject.SetActive(false);
    }
}
