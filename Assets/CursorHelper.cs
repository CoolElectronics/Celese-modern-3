using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHelper : MonoBehaviour
{
    CursorState cursorState = CursorState.Move;

    [SerializeField]
    GameObject move;
    [SerializeField]
    GameObject paint;


    Vector3 moveOrgin;

    Vector3 cameraStored;
    bool dragging = false;
    [SerializeField]
    float cameraSpeed;
    Vector3 paintOrgin;

    AutotilerPreset selectedPreset;

    void Start()
    {

    }
    void OnDrawGizmos()
    {
        if (dragging)
        {
            Gizmos.DrawWireCube(new Vector3((paintOrgin.x + transform.position.x) / 2, (paintOrgin.y + transform.position.y) / 2, 0), new Vector3(Mathf.Abs(paintOrgin.x - transform.position.x) * 2, Mathf.Abs(paintOrgin.y - transform.position.y) * 2, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;
        transform.position = worldPosition;
        switch (cursorState)
        {
            case CursorState.Paint:
                transform.position = new Vector3(Mathf.Round((worldPosition.x - 0.25f) * 2) / 2 + 0.25f, Mathf.Round((worldPosition.y + 0.25f) * 2) / 2 - 0.25f, 0);
                if (Input.GetMouseButtonDown(0))
                {
                    dragging = true;
                    paintOrgin = transform.position;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (dragging)
                    {
                        if ((transform.position - paintOrgin).magnitude > 0.25f)
                        {
                            Debug.Log("Painting Block: " + paintOrgin + " to " + transform.position);
                            Autotiler.i.PaintBlock(paintOrgin, transform.position, selectedPreset);
                        }
                        else
                        {
                            Debug.Log("Painting " + transform.position);
                            Autotiler.i.Paint(transform.position, selectedPreset);
                        }
                        dragging = false;
                    }
                    else
                    {
                        Debug.Log("well, this is awkward");
                    }
                }
                break;
            case CursorState.Move:
                if (dragging)
                {
                    Vector3 camPos = transform.position;
                    camPos.z = -10;
                    moveOrgin.z = -10;
                    Vector3 newCameraPos = cameraStored - (camPos - moveOrgin) / cameraSpeed;
                    newCameraPos.z = -10;
                    Camera.main.transform.position = newCameraPos;
                    if (!Input.GetMouseButton(0))
                    {
                        dragging = false;
                        cameraStored = Camera.main.transform.position;
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    dragging = true;
                    moveOrgin = transform.position;
                }
                break;
        }
    }
    public void UpdateSelectedPreset(AutotilerPreset preset)
    {
        selectedPreset = preset;
        paint.GetComponent<SpriteRenderer>().sprite = selectedPreset.icon;
    }
    public void updateCursorState(CursorState newCursorState)
    {
        cursorState = newCursorState;
        switch (cursorState)
        {
            case CursorState.Move:
                paint.SetActive(false);
                move.SetActive(true);
                break;
            case CursorState.Paint:
                paint.SetActive(true);
                move.SetActive(false);
                break;
        }
    }
}
