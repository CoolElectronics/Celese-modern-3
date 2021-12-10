using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;




// i think this is some of the worst code i've ever written.

[System.Serializable]
public class HierarchyObjectUIDefinition
{

    public List<TMP_InputField> inputFields = new List<TMP_InputField>();
    public List<Toggle> checkboxes = new List<Toggle>();
    HierarchyObject linkedHierarchyObject;

    public string nameDefault = "new room";
    public int exitsDefault = 0;
    public Vector2Int respawnDefault = Vector2Int.zero;
    public Vector2Int camboundsMinDefault = Vector2Int.zero;
    public Vector2Int camboundsMaxDefault = Vector2Int.zero;



    public HierarchyObjectUIDefinition(int type, TMP_InputField x, TMP_InputField y, GameObject viewport, HierarchyObject obk)
    {
        linkedHierarchyObject = obk;
        switch (type)
        {
            case 0:

                // everything is just js now

                inputFields.Add(newInputField(viewport, "name", nameDefault, (val) =>
                {
                    linkedHierarchyObject.ChangeName(val);
                }));

                inputFields.Add(newInputField(viewport, "exits", exitsDefault.ToString(), (val) =>
                {

                }));
                inputFields.Add(newInputField(viewport, "respawn", respawnDefault.x + "," + respawnDefault.y, (val) =>
                {
                    string[] vals = val.Split(",");
                    try
                    {
                        respawnDefault = new Vector2Int(int.Parse(vals[0]), int.Parse(vals[1]));
                        linkedHierarchyObject.ChangeRespawnDefault(respawnDefault);
                    }
                    catch
                    {
                        //haha yeah this is definitely how you write code
                    }

                }));
                inputFields.Add(newInputField(viewport, "cambounds", camboundsMinDefault.x + "," + camboundsMinDefault.y + "," + camboundsMaxDefault.x + "," + camboundsMaxDefault.y, (val) =>
                {
                    string[] vals = val.Split(",");
                    try
                    {
                        camboundsMinDefault = new Vector2Int(int.Parse(vals[0]), int.Parse(vals[1]));
                        camboundsMaxDefault = new Vector2Int(int.Parse(vals[2]), int.Parse(vals[3]));
                        linkedHierarchyObject.ChangeCamBounds(camboundsMinDefault,camboundsMaxDefault);
                    }
                    catch
                    {
                    }
                }));
                break;
        }
    }
    TMP_InputField newInputField(GameObject viewport, string name, string defaultValue, UnityEngine.Events.UnityAction<string> action)
    {
        GameObject fieldobj = LevelEditor.Instantiate(LevelEditor.i.InputValueHierachyUIComponent, Vector3.zero, Quaternion.identity);
        fieldobj.transform.SetParent(viewport.transform);
        fieldobj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        TMP_InputField field = fieldobj.GetComponent<TMP_InputField>();
        field.onValueChanged.AddListener(action);
        field.text = defaultValue;
        fieldobj.transform.localScale = Vector3.one;
        fieldobj.transform.localPosition = Vector3.zero;
        return field;
    }
    public void RemoveAll()
    {
        foreach (TMP_InputField field in inputFields)
        {
            field.onValueChanged.RemoveAllListeners();
            LevelEditor.Destroy(field.gameObject);
        }
        foreach (Toggle toggle in checkboxes)
        {
            toggle.onValueChanged.RemoveAllListeners();
            LevelEditor.Destroy(toggle.gameObject);
        }
    }
}
public enum HierarchyObjectUIComponentType
{
    InputField,
    CheckBox
}