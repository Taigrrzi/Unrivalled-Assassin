using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(canHearSound))]
public class canHearSoundEditor : Editor
{
    public virtual void Start()
    {
        GameObject hearer = (GameObject)target;
    }

    public virtual void OnSceneGUI()
    {
        canHearSound hearer = (canHearSound)target;
        Handles.color = Color.cyan;
        Handles.DrawWireArc(hearer.transform.position, Vector3.forward, Vector3.up, 360, hearer.hearRange);
    }
}