using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BlockSwitchButton : MonoBehaviour
{
    NewCameraController nc;
    [SerializeField]
    int mystate = 0;
    bool xenabled = true;
    void Start()
    {
        nc = NewCameraController.i;
        nc.blockStateUpdateEvent += BlockStateUpdate;
    }
    void BlockStateUpdate(int newstate)
    {
        if (mystate == newstate)
        {
            xenabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            xenabled = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;

        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<playerMovement>() && enabled)
        {
            if (col.gameObject.GetComponent<playerMovement>().dashing)
            {
                nc.blockStateUpdateEvent(mystate);
                NewCameraController.i.Shake(0.3f, 2, 10);

            }
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<playerMovement>() && enabled)
        {
            if (col.gameObject.GetComponent<playerMovement>().dashing)
            {
                nc.blockStateUpdateEvent(mystate);
                NewCameraController.i.Shake(0.3f, 2, 10);

            }
        }
    }
}
