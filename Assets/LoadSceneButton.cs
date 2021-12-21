using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;


public class LoadSceneButton : MonoBehaviour
{
    [SerializeField]
    string SceneName;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(clicked);
    }

    void clicked(){
        SceneManager.LoadScene(SceneName);
    }
}
