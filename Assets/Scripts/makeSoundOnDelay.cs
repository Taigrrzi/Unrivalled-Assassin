using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeSoundOnDelay : MonoBehaviour
{
    public float soundDelay;
    public float soundMod;
    public string soundType;

    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > soundDelay)
        {
            timer = 0;
            gameController.instance.MakeSound(transform.position,gameObject,soundType,soundMod);
        }
    }
}
