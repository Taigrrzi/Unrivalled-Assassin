using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canHearSound : MonoBehaviour
{

    public float hearRange;

    private void Start()
    {
        AddToHearers();
    }

    public void AddToHearers()
    {
        gameController.instance.hearers.Add(gameObject);
    }

    public void HearSound(Vector3 location, string type, string size) {

    }
}
