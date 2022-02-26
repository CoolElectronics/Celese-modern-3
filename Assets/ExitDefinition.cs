// do i really need to oop everything? seems like a bad idea for this case
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitDefinition
{
    public string transferDefault = "?";
    public string posDefault = "0,0";
    public string sizeDefault = "5:1";
    public string playerCoordsDefault = "10,-4";

    HierarchyObjectUIDefinition source;
    RoomTransitionBBox box;
    int index;
    TMPro.TMP_InputField tfield;
    GameObject transferPosPlayer;
    GameObject transitionBoxDisplay;

    float transitionBoxWidth = 0.4f;


    public List<TMPro.TMP_InputField> fields = new List<TMPro.TMP_InputField>();
    Room room;
    public ExitDefinition(HierarchyObjectUIDefinition _source, int _index)
    {
        index = _index;
        source = _source;
        box = new RoomTransitionBBox();
        source.linkedHierarchyObject.room.transitionBoxes.Add(box);
        transferPosPlayer = LevelEditor.Instantiate(LevelEditor.i.transferPosPlayerPrefab, source.linkedHierarchyObject.room.transform);
        transitionBoxDisplay = LevelEditor.Instantiate(LevelEditor.i.transitionBoxDisplay, source.linkedHierarchyObject.room.transform);
        Color rcol = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
        transferPosPlayer.GetComponent<SpriteRenderer>().color = rcol;
        transitionBoxDisplay.GetComponent<SpriteRenderer>().color = rcol;
        room = source.linkedHierarchyObject.room;
        Regenerate();
    }

    public void Delete()
    {
        for (int i = fields.Count - 1; i > -1; i--)
        {
            TMPro.TMP_InputField field = fields[i];
            fields.Remove(field);
            field.onValueChanged.RemoveAllListeners();
            Debug.Log("destruoyed");
            LevelEditor.Destroy(field.gameObject);
        }
        source.linkedHierarchyObject.room.transitionBoxes.Remove(box);
        
        LevelEditor.Destroy(transitionBoxDisplay);
        LevelEditor.Destroy(transferPosPlayer);
    }
    public void Hide()
    {
        for (int i = fields.Count - 1; i > -1; i--)
        {
            TMPro.TMP_InputField field = fields[i];
            fields.Remove(field);
            field.onValueChanged.RemoveAllListeners();
            Debug.Log("hided");
            LevelEditor.Destroy(field.gameObject);
        }
    }
    public void Regenerate()
    {
        tfield = addField(source, "transfer" + index, transferDefault, (val) =>
       {
           transferDefault = val;
           GameObject go = GameObject.Find(val);
           if (go != null && go.GetComponent<Room>() != null)
           {
               if (tfield != null)
               {
                   tfield.GetComponent<UnityEngine.UI.Image>().color = Color.green;
                   box.transferTo = go.GetComponent<Room>();
               }
           }
           else
           {
               if (tfield != null)
               {

                   tfield.GetComponent<UnityEngine.UI.Image>().color = Color.red;
               }
           }
       });
        addField(source, "pos" + index, posDefault, (val) =>
       {
           posDefault = val;
           string[] vals = val.Split(",");
           try
           {
               box.pos = new Vector2(float.Parse(vals[0]) / 2, float.Parse(vals[1]) / 2);
               transitionBoxDisplay.transform.position = box.pos + (Vector2)room.transform.position;
           }
           catch
           {
           }
       });
        addField(source, "size" + index, sizeDefault, (val) =>
       {
           sizeDefault = val;
           string[] vals = val.Split(":");
           try
           {
               Vector3 size = Quaternion.Euler(0,0,90 * float.Parse(vals[1])) * new Vector3(transitionBoxWidth,float.Parse(vals[0]) / 2,1);
            box.size = size;
            transitionBoxDisplay.transform.localScale = size;
           }catch{}
       });
        addField(source, "playercoords" + index, playerCoordsDefault, (val) =>
       {
           playerCoordsDefault = val;
           string[] vals = val.Split(",");
           try
           {
               box.playerCoords = (Vector2)room.transform.position + new Vector2(float.Parse(vals[0])/ 2, float.Parse(vals[1]) / 2);
               transferPosPlayer.transform.position = box.playerCoords;
           }
           catch
           {
           }
       });
    }

    public TMPro.TMP_InputField addField(HierarchyObjectUIDefinition source, string name, string def, UnityEngine.Events.UnityAction<string> action)
    {
        TMPro.TMP_InputField field = source.newInputField(source.viewport, name, def, action);
        fields.Add(field);
        return field;
    }
}