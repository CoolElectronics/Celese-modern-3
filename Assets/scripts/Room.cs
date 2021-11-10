using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    bool showGizmos = false;

    [SerializeField]
    public List<RoomTransitionBBox> transitionBoxes = new List<RoomTransitionBBox>();

    [SerializeField]
    LayerMask lmPlayer;

    [SerializeField]
    public Vector2 defaultRespawn;

    public bool scrollX = false;

    public bool scrollY = false;

    public float minCamX = 0;

    public float minCamY = 0;

    public float maxCamX = 0;

    public float maxCamY = 0;

    public bool lockXscroll = false;

    public Vector2 currentRespawn;

    void Start()
    {
        RoomManager.i.RegisterRoom(this);
        currentRespawn = defaultRespawn;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            foreach (RoomTransitionBBox box in transitionBoxes)
            {
                Gizmos
                    .DrawWireCube((Vector3) box.pos + transform.position,
                    (Vector3) box.size);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (RoomTransitionBBox box in transitionBoxes)
        {
            if (
                Physics2D
                    .OverlapBox(box.pos + (Vector2) transform.position,
                    box.size,
                    0,
                    lmPlayer)
            )
            {
                RoomManager.i.TriggerTransitionBBox(box.transferTo, box);
            }
        }
    }
}
