using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
        public GameManager gameManager;
        public GameObject tailPrefab;
        public Transform tailParent;
        public int maxTailSegments = 10;
        public float bounceBackDistance = 3f;
        public bool hasTail = false;
        public CircleCollider2D playerCollider;
        public Rigidbody2D rb;
        public Vector2 direction;

        public List<GameObject> tailSegments = new List<GameObject>();
        public PlayerMovement playerMovement;
        public Vector2 lastMoveDirection;
        public AudioSource AS;
    
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement reference not set in PlayerCollision script!");
            }

            tailSegments.Add(gameObject);
        
        }

        void Update()
        {
        if (playerMovement != null)
        {
            direction = playerMovement.vel;

            for (int i = tailSegments.Count - 1; i > 0; i--)
            {
                tailSegments[i].transform.position = tailSegments[i - 1].transform.position;
            }

            this.transform.position = new Vector2(
                Mathf.Round(this.transform.position.x) + direction.x,
                Mathf.Round(this.transform.position.y) + direction.y);

            lastMoveDirection = rb.velocity.normalized;

            Vector3 position = transform.position;
            if (Vector3.Distance(position, gameManager.arenaCollider.bounds.center) > gameManager.arenaCollider.radius)
            {
                Vector3 fromCenter = position - gameManager.arenaCollider.bounds.center;
                fromCenter = fromCenter.normalized * gameManager.arenaCollider.radius;
                transform.position = gameManager.arenaCollider.bounds.center + fromCenter;
            }

            Debug.Log("Player Position: " + this.transform.position);
        }
        else
        {
            Debug.LogWarning("PlayerMovement is null, cannot update direction");
        }

            if (gameManager.isGameOver) return;

            if (hasTail == true)
            {
                UpdateTailSegments();
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Food"))
            {
                if (gameManager.foodInstances.Contains(collision.gameObject))
                {
                    gameManager.foodInstances.Remove(collision.gameObject);
                    SoundManager.instance.PlaySound(SoundManager.instance.ScoreSFX);
                }

                AddTailSegment();
                Destroy(collision.gameObject);
                gameManager.AddScore(25);
                SoundManager.instance.PlaySound(SoundManager.instance.PickUpSFX);
                Debug.Log("Food Collected Count: " + gameManager.foodInstances.Count);
            }
            else if (collision.gameObject.CompareTag("Boundary"))
            {
                BounceOffBoundary();
                SoundManager.instance.PlaySound(SoundManager.instance.BoundarySFX);
                Debug.Log("Ricochet");
            }
        }

        void AddTailSegment()
        {
            if (tailSegments.Count >= maxTailSegments)
            {
                Debug.Log("Max Tail Segments reached");
                return;
            }

            GameObject newTailSegment = Instantiate(tailPrefab);
            newTailSegment.transform.position = tailSegments[tailSegments.Count - 1].transform.position;
            tailSegments.Add(newTailSegment);

            Debug.Log("Add Tail Segment: " + newTailSegment.name);
            hasTail = true;
        }

        void UpdateTailSegments()
        {
            if (tailSegments.Count == 0) return;

            Vector3 previousPosition = transform.position;

            foreach (var segment in tailSegments)
            {
                Debug.Log("Updating Tail Segment: " + segment.name);
                Vector3 tempPosition = segment.transform.position;
                segment.transform.position = previousPosition;
                previousPosition = tempPosition;
            }
        }

        void BounceOffBoundary()
        {
           rb.velocity = -lastMoveDirection * rb.velocity.magnitude;
           transform.position += (Vector3)(-lastMoveDirection * bounceBackDistance);
           Debug.Log("Bounce off Boundary: " + transform.position);
        }
    

}
