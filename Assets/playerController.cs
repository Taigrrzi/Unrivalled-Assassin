using UnityEngine;
using System.Collections;
using Pathfinding;

public class playerController : MonoBehaviour
{

    public float speed;
    public float maxSpeed;
    public float acceleration = 0.2f;
    public Rigidbody2D rbd;

    public static playerController instance;


    private void Awake()
    {
        rbd = GetComponent<Rigidbody2D>();
        instance = this;
    }

    void FixedUpdate()
    {

        float leftInput = Input.GetButton("Left") ? 1 : 0;
        float rightInput = Input.GetButton("Right") ? 1 : 0;
        float upInput = Input.GetButton("Up") ? 1 : 0;
        float downInput = Input.GetButton("Down") ? 1 : 0;
        rbd.velocity = Vector2.Lerp(rbd.velocity, new Vector2(speed * (rightInput - leftInput), speed * (upInput - downInput)), acceleration);

        while (rbd.velocity.magnitude > maxSpeed)
        {
            rbd.velocity = rbd.velocity * 0.9f;
        }

        float h = Input.mousePosition.x - Screen.width / 2;
        float v = Input.mousePosition.y - Screen.height / 2;
        float angle = -Mathf.Atan2(h, v) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.fixedDeltaTime * 10);
    }
}