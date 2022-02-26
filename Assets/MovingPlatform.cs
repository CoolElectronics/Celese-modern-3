using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField]
    Vector2 speed;

    Rigidbody2D rb;

    NewCameraController nc;
    Vector2 pos;
    void Start()
    {
        nc = NewCameraController.i;
        rb = GetComponent<Rigidbody2D>();
        pos.x = (int)Mathf.Floor((transform.position.x + nc.screenWidth / 2) / nc.screenWidth);
        pos.y = (int)Mathf.Floor((transform.position.y + nc.screenHeight / 2) / nc.screenHeight);
    }
    void Update()
    {
        rb.velocity = speed;
        if (transform.position.x > pos.x * nc.screenWidth + nc.screenWidth / 2)
        {
            Vector3 rpos = transform.position;
            rpos.x = pos.x * nc.screenWidth - nc.screenWidth / 2;
            transform.position = rpos;
        }
        if (transform.position.x < pos.x * nc.screenWidth - nc.screenWidth / 2)
        {
            Vector3 rpos = transform.position;
            rpos.x = pos.x * nc.screenWidth + nc.screenWidth / 2;
            transform.position = rpos;
        }
    }
}
