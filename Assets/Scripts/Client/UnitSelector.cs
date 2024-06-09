using System;
using UnityEngine;
using Mirror;

public class UnitSelector : MonoBehaviour
{
    public Sprite dragSprite;


    public Vector2 dragStartPosition;

    public Vector2 dragEndPosition;


    public UnitGroup selectedUnits = new UnitGroup();

    private GameObject dragBox;


    [ClientCallback]
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Start drag
            StartSelection();
        }
        if (Input.GetMouseButton(0))
        {
            // Dragging
            ContinueSelection();
        }
        if (Input.GetMouseButtonUp(0))
        {
            // End drag
            EndSelection();
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Right click
            MoveUnits();
        }
    }
    [Client]
    private void StartSelection()
    {
        selectedUnits = new UnitGroup();

        Vector2 mousePosition = GetMousePos();

        dragStartPosition = mousePosition;

        dragBox = new GameObject();
        dragBox.AddComponent<SpriteRenderer>().sprite = dragSprite;
        dragBox.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        dragBox.transform.position = dragStartPosition;
        dragBox.transform.localScale = new Vector3(0, 0, 0);
        dragBox.name = "DragBox";



    }

    [Client]
    private void ContinueSelection()
    {
        Vector2 mousePosition = GetMousePos();

        Vector2 dragBoxSize = mousePosition - dragStartPosition;

        Debug.Log($"Drag box size: {dragBoxSize}, mouse position: {mousePosition}, drag start position: {dragStartPosition}, Raw mouse position: {Input.mousePosition}");

        dragBox.transform.localScale = dragBoxSize;
        dragBox.transform.position = dragStartPosition + dragBoxSize / 2;
    }

    [Client]
    private void EndSelection()
    {
        // Destroy the drag box
        Destroy(dragBox);

        //Get all Units in the area of the box
        Collider2D[] colliders = Physics2D.OverlapAreaAll(dragStartPosition, GetMousePos());
        foreach (Collider2D collider in colliders)
        {

            if (collider.TryGetComponent<Unit>(out Unit unit))
            {
                //If we own this unit, add it to the list
                if (unit.isOwned)
                {
                    selectedUnits.units.Add(unit);
                }

            }
        }
    }

    [Client]
    private void MoveUnits()
    {
        Vector2 mousePosition = GetMousePos();


        NetworkClient.localPlayer.GetComponent<ClientPlayer>().CmdMoveUnits(selectedUnits, mousePosition);

    }

    [Client]
    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;


        //Minus the size of the screen from the mouse position
        mousePos = new Vector3(Screen.width, Screen.height, 0) - mousePos;

        mousePos.z = Camera.main.gameObject.transform.position.z;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePosition;
    }
}