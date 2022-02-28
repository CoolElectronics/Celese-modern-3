using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
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
    public Tilemap triggerMap;
    public Tilemap[] blocksmaps;

    [SerializeField]
    GameObject playerpos;



    float shakeAmp;

    float shakeFreq;

    float shakeFreqTimer;

    float shakeDuration;
    Vector3 oldCameraPosition;
    bool shakeactive = false;

    [SerializeField]
    TMPro.TextMeshProUGUI timertext;
    [SerializeField]
    GameObject batteryPrefab;
    [SerializeField]
    GameObject[] buttonPrefabs;
    [SerializeField]
    GameObject springPrefab;

    private System.Diagnostics.Stopwatch watch;
    public delegate void UpdateBlockState(int newstate);
    public UpdateBlockState blockStateUpdateEvent;

    Dictionary<Vector2Int, Vector3Int> spawnpoints = new Dictionary<Vector2Int, Vector3Int>();

    [SerializeField]
    TileBase spiketilebase;
    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    Image selector;
    [SerializeField]
    List<TMPro.TextMeshProUGUI> pauseTexts;

    MenuItem[] currentItems;

    public bool isPaused = false;
    int selectorPosition = 0;
    public NestedDict<TileBase, List<TileBase>> tileMappings;
    void Awake()
    {
        i = this;

    }
    void Start()
    {


        watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        cam = Camera.main;
        if (playerpos.activeInHierarchy)
        {
            Vector3 ppos = playerpos.transform.position;
            pos.x = (int)Mathf.Floor((ppos.x + screenWidth / 2) / screenWidth);
            pos.y = (int)Mathf.Floor((ppos.y + screenHeight / 2) / screenHeight);
        }
        foreach (Tilemap map in blocksmaps)
        {
            map.gameObject.SetActive(false);
            map.gameObject.SetActive(true);
        }
        triggerMap.gameObject.SetActive(true);
        for (int x = terrainMap.cellBounds.min.x; x < terrainMap.cellBounds.max.x; x++)
        {
            for (int y = terrainMap.cellBounds.min.y; y < terrainMap.cellBounds.max.y; y++)
            {

                Vector3Int tilepos = new Vector3Int(x, y, 0);
                TileBase tile = terrainMap.GetTile(tilepos);
                if (tile != null)
                {
                    if (tileMappings.Get(tile) != null)
                    {
                        List<TileBase> tiles = tileMappings.Get(tile);
                        // terrainMap.SetTile(tilepos, tiles[Random.Range(0, tiles.Count - 1)]);
                    }
                    if (tile.name.Contains("spike"))
                    {
                        // Debug.Log(terrainMap.GetTransformMatrix(tilepos));
                        triggerMap.SetTile(tilepos, tile);
                        triggerMap.SetTransformMatrix(tilepos, terrainMap.GetTransformMatrix(tilepos));
                        terrainMap.SetTile(tilepos, null);

                    }
                    switch (tile.name)
                    {
                        case "1_idle00":

                            Vector2Int spawnpos = Vector2Int.zero;
                            spawnpos.x = (int)Mathf.Floor((x + 22 / 2) / 22);
                            spawnpos.y = (int)Mathf.Floor((y + 17 / 2) / 17);
                            Debug.Log(spawnpos + " " + tilepos);
                            spawnpoints.Add(spawnpos, tilepos);
                            terrainMap.SetTile(tilepos, null);
                            break;
                        case "battery0":
                            Instantiate(batteryPrefab, terrainMap.CellToWorld(tilepos) + Vector3.one / 2, Quaternion.identity);
                            terrainMap.SetTile(tilepos, null);
                            break;
                        case "buttona0":
                            SpawnButton(0, tilepos);
                            break;
                        case "buttonb0":
                            SpawnButton(1, tilepos);
                            break;
                        case "buttonc0":
                            SpawnButton(2, tilepos);
                            break;
                        case "spring0":
                            Quaternion rot = terrainMap.GetTransformMatrix(tilepos).rotation;
                            if (rot.eulerAngles.z == 90 || rot.eulerAngles.z == 180)
                            {
                                Instantiate(springPrefab, terrainMap.CellToWorld(tilepos) - (rot * Vector2.one / 4), rot);

                            }
                            else
                            {
                                Instantiate(springPrefab, terrainMap.CellToWorld(tilepos) + (rot * Vector2.one / 4), rot);

                            }
                            terrainMap.SetTile(tilepos, null);
                            break;



                    }
                }
            }

        }
        RespawnPlayer();
        blockStateUpdateEvent += blockStateUpdate;
        this.Invoke(() => blockStateUpdateEvent(0), 0.1f);
    }
    void SpawnButton(int ind, Vector3Int tilepos)
    {
        Quaternion rot = terrainMap.GetTransformMatrix(tilepos).rotation;
        Instantiate(buttonPrefabs[ind], terrainMap.CellToWorld(tilepos) + stupidRotationOffset(rot), rot);
        terrainMap.SetTile(tilepos, null);

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
    void Update()
    {
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {

                selectorPosition++;
                if (selectorPosition > 2)
                {
                    selectorPosition = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectorPosition--;
                if (selectorPosition < 0)
                {
                    selectorPosition = 2;
                }
            }
            selector.transform.localPosition = new Vector3(-150, 70 - selectorPosition * 70, 0);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                currentItems[selectorPosition].action();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
        {
            if (Input.GetKeyDown(KeyCode.Return)) { if (isPaused) return; };
            void wtf()
            {
                Debug.Log("?");
                isPaused = !isPaused;
                pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
                if (isPaused)
                {
                    Time.timeScale = 0;
                    selectorPosition = 0;
                    RenderMenu(new MenuItem[3] {
                        new MenuItem("Continue", ()=>this.Invoke(wtf,0)),
                        new MenuItem("Options", ()=>{
                            RenderMenu(new MenuItem[3] {
                                new MenuItem("placeholder",()=>{}),
                                new MenuItem("placeholder",()=>{}),
                                new MenuItem("exit",()=>this.Invoke(wtf,0)),
                            });
                }),
                         new MenuItem("Reset Cart", () =>
                         {
                             Time.timeScale = 1;
                             UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                         })
                    });
                }
                else
                {
                    Time.timeScale = 1;
                }
            };
            this.Invoke(wtf, 0);
        }
    }
    void RenderMenu(MenuItem[] items)
    {
        currentItems = items;
        for (int i = 0; i < items.Length; i++)
        {
            pauseTexts[i].text = items[i].name;
        }
    }
    void FixedUpdate()
    {
        timertext.text = TimeFormat(watch.Elapsed);
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
        if (spawnpoints.ContainsKey(pos))
        {
            respawnPos = terrainMap.CellToWorld(spawnpoints[pos]) + new Vector3(.5f, .5f, 0);
        }
        target.transform.position = respawnPos;
        blockStateUpdate(0);
    }
    public void Shake(float _amp, float _freq, float _duration)
    {
        oldCameraPosition = cam.transform.position;
        shakeAmp = _amp;
        shakeFreq = _freq;
        shakeDuration = _duration;
        shakeFreqTimer = _freq;
    }
    string TimeFormat(System.TimeSpan time)
    {
        string clock = "";

        if (time.Minutes < 10)
        {
            clock += "0";
        }
        clock += time.Minutes;
        clock += ":";
        if (time.Seconds < 10)
        {
            clock += "0";
        }
        clock += time.Seconds;
        clock += ".";
        float calcmillis = Mathf.Round(time.Milliseconds / 10);
        if (calcmillis < 10)
        {
            clock += "0";
        }
        clock += calcmillis;

        return clock;
    }
    Vector3 stupidRotationOffset(Quaternion rotation)
    {
        Debug.Log(rotation.eulerAngles.z);
        switch (rotation.eulerAngles.z)
        {
            case 0:
                return new Vector3(0.55f, 0.20f, 0);
            case 90:
                return new Vector3(0.40f, 0.55f, 0);
            case 180:
                return new Vector3(0.55f, 0.35f, 0);
            case 270:
                return new Vector3(0.20f, 0.55f, 0);

        }

        return Vector3.zero;
    }
    public (Tilemap, TileBase) TileGet(Vector3Int pos)
    {
        TileBase tbase;
        tbase = terrainMap.GetTile(pos);
        if (!tbase)
        {
            tbase = triggerMap.GetTile(pos);
            if (tbase)
            {
                return (triggerMap, tbase);
            }
        }
        if (!tbase)
        {
            foreach (Tilemap map in blocksmaps)
            {
                if (map.gameObject.activeInHierarchy)
                {
                    tbase = map.GetTile(pos);
                    if (tbase)
                    {
                        return (map, tbase);
                    }
                }
            }
        }
        return (terrainMap, tbase);
    }
}
