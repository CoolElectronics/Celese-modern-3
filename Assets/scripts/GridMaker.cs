using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    [SerializeField]
    GameObject sh;

    [SerializeField]
    GameObject sv;

    [SerializeField]
    int rows;

    void Start()
    {
        for (int g = -rows; g < rows; g++)
        {
            Instantiate (sv, transform);
            sv.transform.position = new Vector3(g * 18 - 9, 0, 0);
        }
        for (int f = -rows; f < rows; f++)
        {
            Instantiate (sh, transform);
            sh.transform.position = new Vector3(0, f * 11 - 5.5f, 0);
        }
    }
}
