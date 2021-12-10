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
    public void Initialize(int _type, GameObject editObjectPanel, GameObject _levelContainer)
    {
        type = _type;
        levelContainer = _levelContainer;
        definition = new HierarchyObjectUIDefinition(type, editObjectPanel.transform.GetChild(0).GetChild(0).GetComponent<TMP_InputField>(), editObjectPanel.transform.GetChild(1).GetChild(0).GetComponent<TMP_InputField>(), editObjectPanel.transform.GetChild(2).gameObject, this);
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
                room.transitionBoxes = new List<RoomTransitionBBox>();
                break;
        }
    }
    public void ChangeRespawnDefault(Vector2Int respawnDefault)
    {
        if (room)
        {
            room.defaultRespawn = getRealPos(respawnDefault);
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

        Vector3 realPos = Gridloader.i.terrainmap.CellToWorld((Vector3Int)pos);
        realPos += new Vector3(0.5f, 0.5f, 0);
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
