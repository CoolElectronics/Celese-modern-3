using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Vector2Int roomPos = Vector2Int.zero;

    public Room currentRoom;

    public List<Room> rooms;

    [SerializeField]
    Vector2 cameraSize;

    [SerializeField]
    float cameraSmooth;

    public Camera cam;

    public static RoomManager i;

    public Player player;

    float shakeAmp;

    float shakeFreq;

    float shakeFreqTimer;

    float shakeDuration;

    Vector3 oldCameraPosition = Vector3.zero;

    private void Awake()
    {
        i = this;
        rooms = new List<Room>();
    }

    void Start()
    {
        oldCameraPosition.z = -10;
        player = Player.i;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        shakeDuration--;
        if (shakeDuration == 0)
        {
            cam.transform.position = oldCameraPosition;
        }
        if (shakeDuration > 0)
        {
            shakeFreqTimer--;
            if (shakeFreqTimer < 0)
            {
                cam.transform.position =
                    oldCameraPosition +
                    new Vector3(Random.Range(-shakeAmp, shakeAmp),
                        Random.Range(-shakeAmp, shakeAmp),
                        0);
                shakeFreqTimer = shakeFreq;
            }
        }
        else
        {
            cam.transform.position = oldCameraPosition;
        }
        if (currentRoom)
        {
            if (currentRoom.scrollX)
            {
                Vector3 camPos = currentRoom.transform.position;
                camPos.x =
                    Mathf
                        .Clamp(player.transform.position.x,
                        currentRoom.minCamX + currentRoom.transform.position.x,
                        currentRoom.maxCamX + currentRoom.transform.position.x);
                if (
                    player.transform.position.x < currentRoom.minCamX &&
                    currentRoom.lockXscroll
                )
                {
                    camPos.x = currentRoom.transform.position.x;
                }

                camPos.z = -10;
                Debug.Log (camPos);

                Vector3 cvel = Vector3.zero;
                oldCameraPosition =
                    Vector3
                        .SmoothDamp(oldCameraPosition,
                        camPos,
                        ref cvel,
                        cameraSmooth);
            }

            if (currentRoom.scrollY)
            {
                Vector3 camPos = currentRoom.transform.position;
                camPos.y =
                    Mathf
                        .Clamp(player.transform.position.y,
                        currentRoom.minCamY + currentRoom.transform.position.y,
                        currentRoom.maxCamY + currentRoom.transform.position.y);

                camPos.z = -10;
                Debug.Log (camPos);

                Vector3 cvel = Vector3.zero;
                oldCameraPosition =
                    Vector3
                        .SmoothDamp(oldCameraPosition,
                        camPos,
                        ref cvel,
                        cameraSmooth);
            }
        }
    }

    public void RegisterRoom(Room room)
    {
        rooms.Add (room);
    }

    public void Shake(float _amp, float _freq, float _duration)
    {
        oldCameraPosition = cam.transform.position;
        shakeAmp = _amp;
        shakeFreq = _freq;
        shakeDuration = _duration;
        shakeFreqTimer = _freq;
    }

    public void TriggerTransitionBBox(Room room, RoomTransitionBBox box)
    {
        currentRoom = room;
        player.transform.position = box.playerCoords;

        Vector3 pos = room.transform.position;
        pos.z = -10;
        cam.transform.position = pos;
        oldCameraPosition = pos;
    }
}
