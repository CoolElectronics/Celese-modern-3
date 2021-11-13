using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Resettable
{
    bool active = false;

    [SerializeField]
    float speedX;

    [SerializeField]
    float speedY;

    public override void Start()
    {
        base.Start();
    }

    public override void ResetObj()
    {
        base.ResetObj();
    }

    private void FixedUpdate()
    {
        if (active)
        {
            transform.position += new Vector3(speedX, speedY, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 3)
        {
            active = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 3)
        {
            active = false;
            ResetObj();
        }
    }
}
