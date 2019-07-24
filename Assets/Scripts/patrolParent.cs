using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrolParent : MonoBehaviour
{
    public int patrolID;
    public int randomGuardAmount;
    public int randomVictimAmount;
    public Color color = Color.green;
    public bool loop;
    public List<Transform> nodes;

    public void UpdateNodeList() {
        nodes = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            nodes.Add(transform.GetChild(i));
        }
    }
}
