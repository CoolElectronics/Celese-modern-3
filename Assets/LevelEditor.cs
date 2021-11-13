using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    GameObject TilesPanel;

    [SerializeField]
    CursorHelper cursor;


    CursorState cursorState = CursorState.Move;

    public bool playing = false;



    void Start()
    {
        move.onClick.AddListener(movePressed);
        paint.onClick.AddListener(paintPressed);
        play.onClick.AddListener(playPressed);
        save.onClick.AddListener(savePressed);
        expand.onClick.AddListener(expandPressed);

        foreach (AutotilerPreset preset in Autotiler.i.autotilers)
        {
            preset.button.onClick.AddListener(() => selectedAutotiler(preset));
        }
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
        playing = true;
    }
    void savePressed()
    {

    }
    void expandPressed()
    {

    }
    void Update()
    {

    }
    void selectedAutotiler(AutotilerPreset preset)
    {
        Debug.Log("Autotiler " + preset.name + " selected");
        cursor.UpdateSelectedPreset(preset);
    }

}
