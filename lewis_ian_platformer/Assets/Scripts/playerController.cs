using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class playerController : MonoBehaviour
{
    private Rigidbody2D myRB;
    public GameObject bullet;
    private Vector2 velocity;
    public Vector2 respawnPos;
    private Vector2 groundDetection;
    private Quaternion zero;
    public int health = 3;
    public float groundDetectDistance = .1f;
    public float speed = 5;
    public float jumpHeight = 6.5f;
    public float bulletSpeed = 5;
    public float bulletLifespan = .5f;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        respawnPos = new Vector2(0, -2);
        zero = new Quaternion();
    }

    // Update is called once per frame
    void Update()
    {
        // Initialize the point we'll use to check whether or not we're touching the ground every frame

        /*
            Initiazlie the detection point .51 units below the player sprite as default Unity sprites have a height of 1 
            so we want to spawn the ground detection just slightly below our sprite
            so we don't accidentally detect our own sprite as an object we can jump on
        */

        groundDetection = new Vector2(transform.position.x, transform.position.y - .51f);

        if (health <= 0)
        {
            // Forcibly set our player's position back to start if our health drops to or below 0, then reset our health
            transform.SetPositionAndRotation(respawnPos, zero);
            health = 3;
        }

        // Temporarily store previous frame's velocity so we can preserve any movement from last frame if necessary
        velocity = myRB.velocity;

        // Controls player's horizontal velocity based on input
        velocity.x = Input.GetAxisRaw("Horizontal") * speed;

        // If something is within "groundDetectDistance" from the bottom of our sprite (where our groundDetection is initialized at the start of update), Jump!
        if (Input.GetKeyDown(KeyCode.Space) && Physics2D.Raycast(groundDetection, Vector2.down, groundDetectDistance))
        {
            velocity.y = jumpHeight;
        }

        if(Input.GetMouseButtonDown(0))
        {
            // Spawning the bullet based on the prefab I've set for my bullet variable in Unity AT my current player's position.
            GameObject b = Instantiate(bullet, gameObject.transform.position, zero);

            // Ignoring the physics between bullet and player
            Physics2D.IgnoreCollision(b.GetComponent<CircleCollider2D>(), GetComponent<CircleCollider2D>());

            // Calculating where we need too shoot our bullet
            Vector3 lookPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            // Apply the velocity to our bullet so that our bullet reaches our destination
            b.GetComponent<Rigidbody2D>().velocity = new Vector2(lookPos.x * bulletSpeed, lookPos.y * bulletSpeed);

            // Destroy our bullet after "bulletLifespan" amount of seconds (it's a float so it doesn't have to be a whole number)
            Destroy(b, bulletLifespan);
        }

        // Set rigidbody velocity to be our modified or unmodified temp velocity
        myRB.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If object we're colliding with contains the enemy name, subtract one health unit every time our colliders connect
        if (collision.gameObject.name.Contains("enemy"))
        {
            health--;
        }
    }
}
