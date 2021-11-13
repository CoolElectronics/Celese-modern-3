using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resettable : MonoBehaviour
{
    public Vector3 orginPos;

    public Quaternion orginRot;

    public virtual void Start()
    {
        orginPos = transform.position;
        orginRot = transform.rotation;
    }

    public virtual void ResetObj()
    {
        transform.position = orginPos;
        transform.rotation = orginRot;
    }

    void Update()
    {
    }
}
