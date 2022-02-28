using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class newtitlescreen : MonoBehaviour
{
    bool starting = false;
    [SerializeField]
    Image image;
    [SerializeField]
    float Speed;

    void Update()
    {
        if (starting)
        {
            image.color = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1));//image.color. ; new Color(image.color.r - 2, image.color.g - 1, image.color.g - 3);
        }
        if (Input.anyKey)
        {
            starting = true;
            this.Invoke(() => SceneManager.LoadScene("game"), 2);
        }
    }

}

