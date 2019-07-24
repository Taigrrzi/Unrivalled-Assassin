using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(guardController))]
public class FieldOfViewEditor : canHearSoundEditor
{

    public override void OnSceneGUI()
    {
        base.OnSceneGUI();
        guardController fow = (guardController)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow.currentViewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.currentViewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.currentViewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.currentViewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.currentViewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
    }

}