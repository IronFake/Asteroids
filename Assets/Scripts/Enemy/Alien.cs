using System.Net.Security;
using UnityEngine;

public class Alien : MonoBehaviour, IHitHandling
{
    [Header("Points")]
    public int points = 10;

    public float forceSpeed = 2f;

    private Transform playerPos;

    private Rigidbody2D alienRb;
    public GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        alienRb = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        Vector2 dir = playerPos.position - transform.position;
        alienRb.AddForce(dir.normalized * forceSpeed);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        alienRb.rotation = angle;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            Destroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            GameManager.main.UpdateScore(points);
            Destroy(collision.gameObject);
            Destroy();
        }
    }

    public void laserHit()
    {
        Destroy();
        GameManager.main.UpdateScore(points);
    }

    public void Destroy()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
        Destroy(explosion, 5f);

        Destroy(gameObject);
    }

    
}
