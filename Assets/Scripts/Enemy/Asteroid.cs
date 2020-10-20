using UnityEngine;

public class Asteroid : MonoBehaviour, IHitHandling
{
    [Header("Points")]
    public int points = 5;

    public int countOfSmallAsteroids = 2;
    public float forceSpeedRangeOfSmallAsteroids = 2f;

    public int separationStages = 2;
    public int currentStage = 1;
    public float scaleFactor = 0.75f;

    public GameObject asteroidImpactEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            GameManager.main.UpdateScore(points);
            if (currentStage < separationStages) CreateSmallAsteroids(countOfSmallAsteroids);
            Destroy(collision.gameObject);
            Destroy();
        }
    }

    public void laserHit()
    {
        GameManager.main.UpdateScore(points);
        Destroy();
    }

    public void Destroy()
    {
        GameObject explosion = Instantiate(asteroidImpactEffect, transform.position, transform.rotation);
        Destroy(explosion, 5f);
        Destroy(gameObject);
    }

    private void CreateSmallAsteroids(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject smallAsteroid = Instantiate(this.gameObject, transform.position, transform.rotation);
            smallAsteroid.GetComponent<Asteroid>().currentStage += 1;
            smallAsteroid.transform.localScale *= scaleFactor;


            Rigidbody2D rigidbody = smallAsteroid.GetComponent<Rigidbody2D>();
            rigidbody.rotation = Random.Range(0, 180);
            rigidbody.AddRelativeForce(Vector2.up * Random.Range(-forceSpeedRangeOfSmallAsteroids, forceSpeedRangeOfSmallAsteroids),
                ForceMode2D.Impulse);
        }
    }
}
