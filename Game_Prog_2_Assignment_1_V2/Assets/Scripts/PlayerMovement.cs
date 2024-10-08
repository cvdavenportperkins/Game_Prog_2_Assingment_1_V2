using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 0.08f;
    public float acceleration = 0.01f;
    public float deceleration = 0.01f;
    public Vector2 vel;
    public PlayerCollision playerCollision;
    public Rigidbody2D rb;
    public float maxSpeed = 0.12f;
    public GameObject goalGameObject;
    public GameObject arenaGameObject;
    
    public Vector2 arenaCenter;
    public float arenaRadius;

    
    public void InitializePlayer()
    { 
        rb = GetComponent<Rigidbody2D>();
    
        Collider2D goalCollider = goalGameObject.GetComponent<Collider2D>();
        CircleCollider2D arenaCollider = arenaGameObject.GetComponent<CircleCollider2D>();

        arenaCenter = arenaCollider.bounds.center;
        arenaRadius = arenaCollider.radius;

        Vector2 startPosition;
        do
        {
            startPosition = new Vector2(Random.Range(-arenaCollider.radius, arenaCollider.radius), Random.Range(-arenaCollider.radius, arenaCollider.radius));
        } while (Vector2.Distance(startPosition, arenaCollider.bounds.center) > arenaCollider.radius || goalCollider.OverlapPoint(startPosition)) ;

        transform.position = startPosition;
        vel = Vector2.zero;
    }

// Start is called before the first frame update
void Start()                 
    {
       
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDirection = Vector2.zero;                                  //velocity is 0 without input

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x = -1;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x = 1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection.y = 1;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection.y = -1;
        }
       
        if (moveDirection != Vector2.zero)
        {
            vel += moveDirection.normalized * acceleration;
            vel = Vector2.ClampMagnitude(vel, maxSpeed);
        }
        else
        {
            vel = Vector2.Lerp(vel, Vector2.zero, deceleration);
        }

        rb.velocity = vel;

        Vector2 clampedPosition = transform.position;
        if (Vector2.Distance(clampedPosition, arenaCenter) > arenaRadius)
        {
            Vector2 fromCenter = clampedPosition - arenaCenter;
            fromCenter = fromCenter.normalized * arenaRadius;
            clampedPosition = arenaCenter + fromCenter;
            rb.velocity = Vector2.zero;
        }

        transform.position = clampedPosition;

        Debug.Log("Player Position: " + transform.position);

        if (playerCollision.hasTail == true)
        {
            moveSpeed = 0.12f * Time.deltaTime;
        }
    
    }
}
