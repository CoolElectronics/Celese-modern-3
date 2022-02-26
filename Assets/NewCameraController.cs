using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class NewCameraController : MonoBehaviour
{
    public static NewCameraController i;
    [SerializeField]
    GameObject target;

    public Vector2Int pos;

    public float screenWidth;
    public float screenHeight;

    Camera cam;
    [SerializeField]
    Vector2 defaultRespawnPos;

    [SerializeField]
    public Tilemap terrainMap;

    [SerializeField]
    public Tilemap signalMap;
    public Tilemap hazardMap;
    public Tilemap[] blocksmaps;

    [SerializeField]
    GameObject playerpos;



    float shakeAmp;

    float shakeFreq;

    float shakeFreqTimer;

    float shakeDuration;
    Vector3 oldCameraPosition;
    bool shakeactive = false;




    public delegate void UpdateBlockState(int newstate);
    public UpdateBlockState blockStateUpdateEvent;
    void Awake()
    {
        i = this;

    }
    void Start()
    {
        cam = Camera.main;
        if (playerpos.activeInHierarchy)
        {
            Vector3 ppos = playerpos.transform.position;
            pos.x = (int)Mathf.Floor((ppos.x + screenWidth / 2) / screenWidth);
            pos.y = (int)Mathf.Floor((ppos.y + screenHeight / 2) / screenHeight);
        }

        RespawnPlayer();
        blockStateUpdateEvent += blockStateUpdate;
        blockStateUpdateEvent(0);
    }
    void blockStateUpdate(int newstate)
    {
        int index = 0;
        foreach (Tilemap t in blocksmaps)
        {
            if (index == newstate)
            {
                t.gameObject.SetActive(true);
            }
            else
            {
                t.gameObject.SetActive(false);
            }
            index++;
        }
    }

    void FixedUpdate()
    {
        if (target.transform.position.y > (pos.y * screenHeight) - 0.5 + screenHeight / 2)
        {
            pos.y++;
            RespawnPlayer();
        }
        if (target.transform.position.x > (pos.x * screenWidth) - 0.5 + screenWidth / 2)
        {

            pos.x++;
            RespawnPlayer();
        }
        if (target.transform.position.y < (pos.y * screenHeight) + 0.5 - screenHeight / 2)
        {
            Player.i.Kill(Vector2.up);
        }
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
            cam.transform.position = new Vector3(pos.x * screenWidth, pos.y * screenHeight, -10);
        }
    }

    public void RespawnPlayer()
    {
        target.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Vector2 respawnPos = defaultRespawnPos + new Vector2(pos.x * screenWidth, pos.y * screenHeight);
        Vector3Int start = new Vector3Int(pos.x * 24 - 12, pos.y * 17 - 8, 0);
        // Debug.Log(start.x + " " + start.y);
        for (int x = start.x; x < start.x + 24; x++)
        {
            for (int y = start.y; y < start.y + 19; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = signalMap.GetTile(pos);
                if (tile != null)
                {
                    // Debug.Log(tile.name);
                    if (tile.name == "1_idle00")
                    {
                        // Debug.Log("sending to " + pos + " " + signalMap.CellToWorld(pos));
                        respawnPos = signalMap.CellToWorld(pos) + new Vector3(.5f, .5f, 0);
                    }
                }
            }
        }
        Debug.Log("repositioning");
        target.transform.position = respawnPos;
    }
    public void Shake(float _amp, float _freq, float _duration)
    {
        oldCameraPosition = cam.transform.position;
        shakeAmp = _amp;
        shakeFreq = _freq;
        shakeDuration = _duration;
        shakeFreqTimer = _freq;
    }
}
