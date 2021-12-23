using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

// don't touch. it works.
public class LevelEditor : MonoBehaviour
{
    [SerializeField]
    Button move;
    [SerializeField]
    Button paint;
    [SerializeField]
    Button play;
    [SerializeField]
    Button save;
    [SerializeField]
    Button expand;

    [SerializeField]
    Button raster;
    [SerializeField]
    Button newObjectButton;
    [SerializeField]

    GameObject TilesPanel;

    [SerializeField]
    CursorHelper cursor;
    [SerializeField]
    GameObject levelContainer;
    [SerializeField]
    GameObject levelCodePanel;
    [SerializeField]
    TMP_InputField levelCodeText;
    [SerializeField]
    Button closeCodeGui;
    [SerializeField]
    GameObject newObjectPanel;
    [SerializeField]
    Button newObjectClose;
    [SerializeField]
    Button newObjectCreate;
    [SerializeField]
    TMP_Dropdown newObjectType;
    [SerializeField]
    Button load;

    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    RoomManager roomManager;
    [SerializeField]
    GameObject hierarchyPanel;

    [SerializeField]
    TextMeshProUGUI posDisplay;
    [SerializeField]
    GameObject HierarchyObjectPrefab;
    public GameObject InputValueHierachyUIComponent;

    Player player;



    CursorState cursorState = CursorState.Move;

    public bool playing = false;

    [SerializeField]
    GameObject editObjectPanel;
    [SerializeField]
    TMP_InputField editObjectPosX;
    [SerializeField]
    TMP_InputField editObjectPosY;
    [SerializeField]
    Button editObjectClose;

    HierarchyObject activeHierarchyObject;


    public static LevelEditor i;
    List<HierarchyObject> hierarchyObjects;
    
    public GameObject transferPosPlayerPrefab;
    public GameObject transitionBoxDisplay;
    public Room mainRoom;

    void Awake()
    {
        i = this;
    }
    void Start()
    {
        move.onClick.AddListener(movePressed);
        paint.onClick.AddListener(paintPressed);
        play.onClick.AddListener(playPressed);
        save.onClick.AddListener(savePressed);
        expand.onClick.AddListener(expandPressed);
        raster.onClick.AddListener(rasterPressed);
        load.onClick.AddListener(loadPressed);
        closeCodeGui.onClick.AddListener(closeCodeGuiPressed);
        newObjectButton.onClick.AddListener(newObjectButtonPressed);
        newObjectClose.onClick.AddListener(newObjectClosePressed);
        newObjectCreate.onClick.AddListener(newObjectCreatePressed);
        editObjectClose.onClick.AddListener(editObjectClosePressed);
        hierarchyObjects = new List<HierarchyObject>();

        foreach (AutotilerPreset preset in Autotiler.i.autotilers)
        {
            preset.button.onClick.AddListener(() => selectedAutotiler(preset));
        }
    }
    public void NewRoom(HierarchyObject r){
        if (mainRoom == null){
            mainRoom = r.room;
            roomManager.currentRoom = r.room;
        }
    }
    void loadPressed()
    {
        LevelHash hash = new LevelHash();
        hash.isLevelEditor = true;
        hash.leveldata = levelCodeText.text;
        Gridloader.i.LoadLevel(hash);
    }
    void newObjectButtonPressed()
    {
        newObjectPanel.SetActive(true);
    }
    void newObjectClosePressed()
    {
        newObjectPanel.SetActive(false);
    }
    void editObjectClosePressed()
    {
        editObjectPanel.SetActive(false);
        activeHierarchyObject.Deactivate();
    }
    void newObjectCreatePressed()
    {
        newObjectClosePressed();

        HierarchyObject hierarchyObj = Instantiate(HierarchyObjectPrefab, Vector3.zero, Quaternion.identity).GetComponent<HierarchyObject>();
        hierarchyObj.Initialize(newObjectType.value,editObjectPanel,levelContainer);
        hierarchyObj.transform.SetParent(hierarchyPanel.transform.GetChild(0).GetChild(0));
        hierarchyObj.transform.localScale = Vector3.one;
        hierarchyObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        hierarchyObj.GetComponent<Button>().onClick.AddListener(() => { hierarchyObjButtonPressed(hierarchyObj); });
        hierarchyObjects.Add(hierarchyObj);



    }

    public void MakeRoom(string name, Vector3 pos, Vector2 defaultRespawn, bool lockXscroll, bool sx, bool sy, float mcx,float mcy,float Mcx, float Mcy, List<RoomTransitionBBoxSerializable> boxes){
        HierarchyObject hierarchyObj = Instantiate(HierarchyObjectPrefab, Vector3.zero, Quaternion.identity).GetComponent<HierarchyObject>();
        hierarchyObj.Initialize(0,editObjectPanel,levelContainer, true);
        hierarchyObj.transform.SetParent(hierarchyPanel.transform.GetChild(0).GetChild(0));
        hierarchyObj.transform.localScale = Vector3.one;
        hierarchyObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        hierarchyObj.GetComponent<Button>().onClick.AddListener(() => { hierarchyObjButtonPressed(hierarchyObj); });
        hierarchyObjects.Add(hierarchyObj);

        HierarchyObjectUIDefinition def = hierarchyObj.definition;
        hierarchyObj.Activate();
        def.inputFields[0].text = pos.x / HierarchyObjectUIDefinition.roomWidth + "," + pos.y / HierarchyObjectUIDefinition.roomHeight;
        def.inputFields[1].text = name;
        def.inputFields[2].text = boxes.Count.ToString();
        def.inputFields[3].text = defaultRespawn.x + "," + defaultRespawn.y;
        Vector2Int m = HierarchyObject.getFakePos(new Vector3(mcx,mcy,0));
        Vector2Int M = HierarchyObject.getFakePos(new Vector3(Mcx,Mcy,0));

        //help
        def.inputFields[4].text = m.x + "," + m.y + "," + M.x + "," + M.y;
        for (int i = 0; i < boxes.Count;  i++){
            ExitDefinition exit = def.exits[i];
            exit.fields[0].text = boxes[i].transferTo;
            exit.fields[1].text = boxes[i].posx * 2 + "," + boxes[i].posy * 2;
            if (boxes[i].sizex > boxes[i].sizey){
                exit.fields[2].text = boxes[i].sizex + ":1";
            }else{
                exit.fields[2].text = boxes[i].sizey + ":0";
            }
            exit.fields[3].text = (boxes[i].playerCoordsx * 2 - hierarchyObj.room.transform.position.x) + "," + (boxes[i].playerCoordsy * 2 - hierarchyObj.room.transform.position.y);

        }

        hierarchyObj.Deactivate();

    }
    void hierarchyObjButtonPressed(HierarchyObject hierarchyObject)
    {
        if (!editObjectPanel.activeSelf)
        {
            editObjectPanel.SetActive(true);
            activeHierarchyObject = hierarchyObject;
            hierarchyObject.Activate();
        }
    }
    void closeCodeGuiPressed()
    {
        levelCodePanel.SetActive(false);
    }
    void movePressed()
    {
        cursorState = CursorState.Move;
        cursor.updateCursorState(cursorState);
    }
    void paintPressed()
    {
        cursorState = CursorState.Paint;
        cursor.updateCursorState(cursorState);
    }
    void playPressed()
    {
        if (!playing)
        {
            if (player == null)
            {

                player = Instantiate(playerPrefab, transform.position, Quaternion.identity).GetComponent<Player>();
                roomManager.currentRoom = mainRoom;
                roomManager.active = true;
                roomManager.player = player;
                player.transform.position = mainRoom.currentRespawn;
            }
            play.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "stop";
            playing = true;
        }
        else
        {
            roomManager.active = false;
            Destroy(player.gameObject);
            play.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "play";
            playing = false;
        }
    }
    void rasterPressed()
    {
        cursorState = CursorState.Raster;
        cursor.updateCursorState(cursorState);
    }
    void savePressed()
    {
        SaveLevel();
    }
    public void SaveLevel()
    {
        Tilemap terrainmap = Gridloader.i.terrainmap;
        Tilemap hazardmap = Gridloader.i.hazardmap;
        Tilemap bgmap = Gridloader.i.bgmap;
        MapData md = new MapData();
        md.objs = new List<LevelObject>();
        foreach (Transform trans in levelContainer.transform)
        {


            GameObject obj = trans.gameObject;
            if (obj.GetComponent<Room>())
            {
                Room room = obj.GetComponent<Room>();
                LevelObject serializableObject = new LevelObject();
                serializableObject.type = "Room";
                serializableObject.positionx = trans.position.x;
                serializableObject.positiony = trans.position.y;
                serializableObject.name = room.name;
                serializableObject.lockXscroll = room.lockXscroll;
                serializableObject.maxCamX = room.maxCamX;
                serializableObject.maxCamY = room.maxCamY;
                serializableObject.minCamX = room.minCamX;
                serializableObject.minCamY = room.minCamY;
                serializableObject.scrollX = room.scrollX;
                serializableObject.scrollY = room.scrollY;
                serializableObject.transitionBoxes = new List<RoomTransitionBBoxSerializable>();
                foreach (RoomTransitionBBox box in room.transitionBoxes)
                {
                    RoomTransitionBBoxSerializable serializableBox = new RoomTransitionBBoxSerializable();
                    serializableBox.playerCoordsx = box.playerCoords.x;
                    serializableBox.playerCoordsy = box.playerCoords.y;
                    serializableBox.sizex = box.size.x;
                    serializableBox.sizey = box.size.y;
                    serializableBox.posx = box.pos.x;
                    serializableBox.posy = box.pos.y;

                    serializableBox.playerCoordsx = box.playerCoords.x;
                    serializableBox.playerCoordsy = box.playerCoords.y;
                    serializableBox.transferTo = box.transferTo.name;
                    serializableObject.transitionBoxes.Add(serializableBox);
                }

                serializableObject.defaultrespawnx = room.defaultRespawn.x;
                serializableObject.defaultrespawny = room.defaultRespawn.y;

                md.objs.Add(serializableObject);
            }
            else if (obj.GetComponent<Battery>())
            {
                LevelObject serializableObject = new LevelObject();
                serializableObject.type = "Battery";
                serializableObject.orginPosx = trans.position.x;
                serializableObject.orginPosy = trans.position.y;

                md.objs.Add(serializableObject);
            }
        }

        string[,] terrain = new string[terrainmap.cellBounds.size.x, terrainmap.cellBounds.size.y];
        for (int x = terrainmap.cellBounds.min.x; x < terrainmap.cellBounds.max.x; x++)
        {
            for (int y = terrainmap.cellBounds.min.y; y < terrainmap.cellBounds.max.y; y++)
            {
                TileBase tile = terrainmap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    md.terrainmap.Add(tile.name + "|" + terrainmap.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        }

        string[,] hazard = new string[hazardmap.cellBounds.size.x, hazardmap.cellBounds.size.y];
        for (int x = hazardmap.cellBounds.min.x; x < hazardmap.cellBounds.max.x; x++)
        {
            for (int y = hazardmap.cellBounds.min.y; y < hazardmap.cellBounds.max.y; y++)
            {
                TileBase tile = hazardmap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    md.hazardmap.Add(tile.name + "|" + hazardmap.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        }
        string[,] bg = new string[bgmap.cellBounds.size.x, bgmap.cellBounds.size.y];
        Debug.Log(bgmap.cellBounds.size.x + " : " + bgmap.cellBounds.size.y);
        for (int x = bgmap.cellBounds.min.x; x < bgmap.cellBounds.max.x; x++)
        {
            for (int y = bgmap.cellBounds.min.y; y < bgmap.cellBounds.max.y; y++)
            {
                TileBase tile = bgmap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {

                    md.bgmap.Add(tile.name + "|" + bgmap.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation.eulerAngles.z + "|" + x + "|" + y);
                }
            }
        }
        levelCodePanel.SetActive(true);
        levelCodeText.text = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(md)));
    }

    void expandPressed()
    {

    }
    void Update()
    {
        posDisplay.text = "pos : (" + Mathf.Round(cursor.transform.position.x) + ", " + Mathf.Round(cursor.transform.position.y) + ")";
    }
    void selectedAutotiler(AutotilerPreset preset)
    {
        cursor.UpdateSelectedPreset(preset);
    }

}
