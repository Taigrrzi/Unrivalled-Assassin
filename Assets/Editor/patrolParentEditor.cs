using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(patrolParent))]
public class patrolParentEditor : Editor
{

    void OnSceneGUI()
    {
        Transform parent = ((patrolParent)target).transform;
        Handles.color = Color.green;

        for (int i = 0; i < parent.childCount-1; i++)
        {
            Handles.DrawSolidDisc(parent.GetChild(i).position,Vector3.forward,0.5f);
            Handles.DrawLine(parent.GetChild(i).position, parent.GetChild(i+1).position);
        }
        Handles.DrawSolidDisc(parent.GetChild(parent.childCount-1).position, Vector3.forward, 0.5f);
    }

}