using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HierarchyObject : MonoBehaviour
{
    [SerializeField]
    GameObject roomPrefab;
    GameObject levelContainer;
    public int type;
    new public string name;

    public HierarchyObjectUIDefinition definition;

    public Room room;

    GameObject editObjectPanel;
    public void Initialize(int _type, GameObject _editObjectPanel, GameObject _levelContainer)
    {
        type = _type;
        editObjectPanel = _editObjectPanel;
        levelContainer = _levelContainer;
        definition = new HierarchyObjectUIDefinition();
        
        switch (type)
        {
            case 0:
                GameObject instantiated = Instantiate(roomPrefab, levelContainer.transform);
                //instantiated.name = name;
                //instantiated.transform.position = new Vector3(obj.positionx, obj.positiony, 0);
                room = instantiated.GetComponent<Room>();
                //room.defaultRespawn = new Vector2(obj.defaultrespawnx, obj.defaultrespawny);
                // room.lockXscroll = obj.lockXscroll;
                // room.scrollX = obj.scrollX;
                // room.scrollY = obj.scrollY;
                // room.minCamX = obj.minCamX;
                // room.minCamY = obj.minCamY;
                // room.maxCamX = obj.maxCamX;
                // room.maxCamY = obj.maxCamY;
                LevelEditor.i.NewRoom(this);
                room.transitionBoxes = new List<RoomTransitionBBox>();
                break;
        }
    }
    public void Activate(){
        definition.Initalize(type, editObjectPanel.transform.GetChild(0).gameObject, this);
    }
    public void Deactivate(){
        definition.RemoveAll();
    }
    public void changePos(Vector3 pos){
        room.transform.position = pos;
    }
    public void ChangeRespawnDefault(Vector2Int respawnDefault)
    {
        if (room)
        {
            room.defaultRespawn = getRealPos(respawnDefault);
            room.transform.GetChild(1).localPosition = getRealPos(respawnDefault);
        }
    }
    public void ChangeCamBounds(Vector2Int min, Vector2Int max)
    {
        if (room)
        {
            room.minCamX = getRealPos(min).x;
            room.minCamY = getRealPos(min).y;
            room.maxCamX = getRealPos(max).x;
            room.maxCamY = getRealPos(max).y;
        }
    }
    public static Vector3 getRealPos(Vector2Int pos)
    {

        Vector3 realPos = (Vector3Int)pos;

        /// dont. even. ask.
        realPos -= new Vector3(-0.25f, -0.25f, 0);
        return realPos;
    }
    public void ChangeName(string _name)
    {
        if (room)
        {
            name = _name;
            room.gameObject.name = name;
            transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = name + "(room)";
        }
    }
}
