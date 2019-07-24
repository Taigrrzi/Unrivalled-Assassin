using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

//[CanEditMultipleObjects]
[CustomEditor(typeof(patrolParent))]
public class patrolParentEditor : Editor
{

    void OnSceneGUI()
    {
        Transform parent = ((patrolParent)target).transform;
        Handles.color = parent.GetComponent<patrolParent>().color;

        for (int i = 0; i < parent.childCount-1; i++)
        {
            Handles.DrawSolidDisc(parent.GetChild(i).position,Vector3.forward,0.5f);
            Handles.DrawLine(parent.GetChild(i).position, parent.GetChild(i+1).position);
        }
        if (parent.childCount > 0)
        {
            Handles.DrawSolidDisc(parent.GetChild(parent.childCount - 1).position, Vector3.forward, 0.5f);
            if (parent.GetComponent<patrolParent>().loop)
            {
                Handles.DrawLine(parent.GetChild(0).position, parent.GetChild(parent.childCount-1).position);
            }
        }
        HandleKeyboard(parent);
    }
    private void HandleKeyboard(Transform parent)
    {
        Event current = Event.current;
        if (current.type != EventType.KeyDown)
        {
            return;
        }

        switch (current.keyCode)
        {
            case KeyCode.P:
                Vector2 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                if (parent.childCount>1&&Vector2.Distance(mousePos,parent.GetChild(0).position)<1)
                {
                    parent.GetComponent<patrolParent>().loop = !parent.GetComponent<patrolParent>().loop;
                } else
                {
                    GameObject newNode = new GameObject("Patrol Node " + parent.childCount);
                    newNode.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
                    newNode.transform.parent = parent;
                }
                break;
            case KeyCode.L:
                if (parent.childCount>0)
                {
                    DestroyImmediate(parent.GetChild(parent.childCount - 1).gameObject);
                }
                break;
            default:
                break;
        }
    }
}