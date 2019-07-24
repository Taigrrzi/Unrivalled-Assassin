using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

//[CanEditMultipleObjects]
[CustomEditor(typeof(levelParts))]
public class levelPartsEditor : Editor
{

    void OnSceneGUI()
    {
        Transform parent = ((levelParts)target).transform;
        //List<Color> colorSeq = new List<Color>() {Color.green,Color.red,Color.blue,Color.yellow};


        List<Transform> patrolParents = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).GetComponent<patrolParent>() != null) {
                patrolParents.Add(parent.GetChild(i));
            }
        }
        for (int i = 0; i < patrolParents.Count; i++)
        {
            //Handles.color = colorSeq[i % colorSeq.Count];
            Transform patrolParent = patrolParents[i];
            Handles.color = patrolParent.GetComponent<patrolParent>().color;
            for (int j = 0; j < patrolParent.childCount - 1; j++)
            {
                Handles.DrawSolidDisc(patrolParent.GetChild(j).position, Vector3.forward, 0.5f);
                Handles.DrawLine(patrolParent.GetChild(j).position, patrolParent.GetChild(j + 1).position);
            }
            if (patrolParent.childCount > 0)
            {
                Handles.DrawSolidDisc(patrolParent.GetChild(patrolParent.childCount - 1).position, Vector3.forward, 0.5f);
                if (patrolParent.GetComponent<patrolParent>().loop)
                {
                    Handles.DrawLine(patrolParent.GetChild(0).position, patrolParent.GetChild(patrolParent.childCount - 1).position);
                }
            }
        }
    }
}