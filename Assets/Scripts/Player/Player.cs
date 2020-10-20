using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 10;

    public GameObject explosionPrefab;

    private Vector2 movement;
    private Vector2 lookDir;

    private Rigidbody2D playerRb;

    //Declare bounds of player movement
    private float leftBorder;
    private float rightBorder;
    private float topBorder;
    private float bottomBorder;

    private void Awake()
    {
        float halfOfWidthPlayerSptire = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float halfOfHeightPlayerSptire = GetComponent<SpriteRenderer>().bounds.size.y / 2;

        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        leftBorder = -screenBounds.x + halfOfWidthPlayerSptire;
        rightBorder = screenBounds.x - halfOfWidthPlayerSptire;
        topBorder = screenBounds.y - halfOfHeightPlayerSptire;
        bottomBorder = -screenBounds.y + halfOfHeightPlayerSptire;
    }

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.main.isPlaying == true)
        {
            movement.x = CrossPlatformInputManager.GetAxis("Horizontal");
            movement.y = CrossPlatformInputManager.GetAxis("Vertical"); 
        }
#if UNITY_STANDALONE

        lookDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);

#elif MOBILE_INPUT

        lookDir.x = CrossPlatformInputManager.GetAxis("Horizontal_2");
        lookDir.y = CrossPlatformInputManager.GetAxis("Vertical_2");

#endif
    }

    void FixedUpdate()
    {
        playerRb.AddForce(movement  * moveSpeed);

#if UNITY_STANDALONE
        Vector2 viewDir = lookDir - playerRb.position;
        float angle = Mathf.Atan2(viewDir.y, viewDir.x) * Mathf.Rad2Deg - 90f;
        playerRb.rotation = angle;
#endif

#if MOBILE_INPUT
        if (lookDir != Vector2.zero)
        {
            Vector2 posVec2 = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPos = lookDir + posVec2;
            transform.up = targetPos - posVec2;
        }
#endif
    }

    private void LateUpdate()
    {
        // Check that player doesn't left the boundaries of the screen
        Vector2 playerPos = transform.position;
        
        if (playerPos.x >= rightBorder || playerPos.x <= leftBorder)
        {
            playerPos.x = Mathf.Clamp(playerPos.x, leftBorder, rightBorder);
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);

            transform.position = playerPos;
        }
        
        if (playerPos.y >= topBorder || playerPos.y <= bottomBorder)
        {
            playerPos.y = Mathf.Clamp(playerPos.y, bottomBorder, topBorder);
            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);

            transform.position = playerPos;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Alien") ||
            collision.gameObject.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            GameObject explosion = Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
            Destroy(explosion, 5f);

            GameManager.main.GameOver();
        }
    }
}
