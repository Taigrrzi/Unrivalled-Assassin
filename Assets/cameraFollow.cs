using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cameraFollow : MonoBehaviour
{

    public List<GameObject> targets;
    public float zoomSpeed;
    public float intendedZoom;
    public float maxZoom = 16;
    public float minZoom ;
    public Vector2 mouseScreenPosition;
    public Vector2 mouseWorldPosition;
    public Vector3 intendedPosition;
    
    public List<Vector3> positions;

    private void Start()
    {
        if (targets.Count == 0)
        {
            //target =;
        }
    }
    // Use this for initialization
    // Update is called once per frame
    void FixedUpdate()
    {
        intendedZoom -= zoomSpeed * Input.GetAxis("Mouse ScrollWheel");
        if (!targets.TrueForAll(p => IsOnCamera(p.transform.position))) {
            minZoom += 0.2f;
        } else
        {
            minZoom -= 0.1f;
        }
        if (minZoom < 2) {
            minZoom = 2;
        }
        if (intendedZoom < minZoom)
        {
            intendedZoom = minZoom;
        }
        if (intendedZoom > maxZoom)
        {
            intendedZoom = maxZoom;
        }
        intendedPosition = Vector3.zero;
        positions = new List<Vector3>();
        foreach (GameObject item in targets)
        {
            positions.Add(item.transform.position);
        }

        //transform.position = new Vector3 (target.transform.position.x, target.transform.position.y, Mathf.Lerp(transform.position.z,intendedZoom,0.2f));
        if (Input.GetMouseButton(1))
        {
            mouseScreenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0f));
            positions.Add(mouseWorldPosition);
            //intendedPosition = new Vector3((target.transform.position.x + mouseWorldPosition.x) / 2, (target.transform.position.y + mouseWorldPosition.y) / 2, -1);
        }
        foreach (Vector3 item in positions)
        {
            intendedPosition += item;
        }
        intendedPosition = intendedPosition / positions.Count;
        intendedPosition.z = -1;


        transform.position = Vector3.Lerp(transform.position, intendedPosition, Mathf.Abs(((17 - intendedZoom) / 16) - (intendedZoom / (intendedZoom + 1)) / 4) + 0.1f);
        //transform.position = intendedPosition ;
        //Mathf.Abs(((17-intendedZoom)/16)-(intendedZoom/(intendedZoom+1))/2)
        //Debug.Log (GetComponent<Camera> ().orthographicSize);
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, intendedZoom, 0.2f);
    }
    public bool IsOnCamera(Vector3 pos) {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(pos);
        return (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1);
    }
}