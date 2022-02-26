using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ButtonScript : MonoBehaviour
{
    [SerializeField]
    SAction action;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<playerMovement>())
        {
            if (col.gameObject.GetComponent<playerMovement>().dashing)
            {
                action.Invoke();
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void Unpress()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
