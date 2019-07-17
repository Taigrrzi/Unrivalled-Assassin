using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
class runInEditor : MonoBehaviour
{
    static runInEditor()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        
        gameController controller = GameObject.Find("GameController").GetComponent<gameController>();
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Player"))
        {
            controller.players.Add(item);
        }
    }
}