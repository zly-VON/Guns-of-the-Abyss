using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;


    #region Editor Code
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    // Initialise node
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    // Draw node with the nodestyle
    public void Draw(GUIStyle nodeStyle)
    {
        // Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);

        // Start Region To Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();

        // Display a pop-up using the RoomNodeType name values that can be selected
        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
        roomNodeType = roomNodeTypeList.list[selection];
        
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    // Populate a string array with the room node types to display that can be selected
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];
        
        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    // Process events for the node
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default: break;
        }
    }

    // Process mouse down events
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        // right click down
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    // Process left click down event
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        // Toggle node selection
        if (isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    // Process right click down
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    // Process mouse up event
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // If left click up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    /// Process left click up event
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    // Process mouse drag event
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process left click drag event
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    // Process left mouse drag event
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    // Drag node
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    // Add childID to the node
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        childRoomNodeIDList.Add(childID);
        return true;
    }

    // Add aprentID to the node
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }
    

#endif
    #endregion Editor Code

}
