using UnityEngine;
using System.Collections;
using Pathfinding;

public class playerController : MonoBehaviour
{

    public float speed;
    public float maxSpeed;
    public float acceleration = 0.2f;
    private Rigidbody2D rbd;
    public int playerNumber;

    private string upMove;
    private string downMove;
    private string rightMove;
    private string leftMove;


    private void Awake()
    {
        rbd = GetComponent<Rigidbody2D>();
        Camera.main.GetComponent<cameraFollow>().targets.Add(gameObject);
    }

    private void Start()
    {
        if (gameController.instance!=null&&gameController.instance.players.Contains(gameObject))
        {
            gameController.instance.players.Add(gameObject);
        }
        switch (playerNumber)
        {
            case 0:
                upMove = "up";
                leftMove = "left";
                rightMove = "right";
                downMove = "down";
                break;
            case 1:
                upMove = "w";
                leftMove = "a";
                rightMove = "d";
                downMove = "s";
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {

        float leftInput = Input.GetKey(leftMove) ? 1 : 0;
        float rightInput = Input.GetKey(rightMove) ? 1 : 0;
        float upInput = Input.GetKey(upMove) ? 1 : 0;
        float downInput = Input.GetKey(downMove) ? 1 : 0;
        //float leftInput = Input.GetButton(leftMove) ? 1 : 0;
        //float rightInput = Input.GetButton(rightMove) ? 1 : 0;
        //float upInput = Input.GetButton(upMove) ? 1 : 0;
        //float downInput = Input.GetButton(downMove) ? 1 : 0;
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