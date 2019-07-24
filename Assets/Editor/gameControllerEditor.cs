using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(gameController))]
public class gameControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        gameController myScript = (gameController)target;
        if (GUILayout.Button("Rename Patrol Nodes"))
        {
            myScript.RenamePatrolNodes();
        }
    }
}