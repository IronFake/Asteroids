using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityStandardAssets.CrossPlatformInput;

public class Shooting : MonoBehaviour
{
    [Header("Laser")]
    public float laserTime = 2f;
    public GameObject laserCharge;
    public LineRenderer laser;
    public AudioSource bulletSound;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public AudioSource laserSound;
#if MOBILE_INPUT
    public float fireRate = 1;
    private float fireCountdown;
#endif

    private bool isLaserActive = false;

    void Update()
    {
        if (GameManager.main.isPlaying)
        {
#if UNITY_STANDALONE

            if (Input.GetMouseButtonDown("Fire1") && !isLaserActive)
            {
                ShootBullet();
            }

#elif MOBILE_INPUT
            if (!isLaserActive)
            {
                if (fireCountdown <= 0f)
                {
                    ShootBullet();
                    fireCountdown = 1f / fireRate;
                }
                fireCountdown -= Time.deltaTime;
            }
#endif

            if (CrossPlatformInputManager.GetButtonDown("Fire2") && isLaserActive == false && GameManager.main.currentLaserShots > 0)
            {
                GameManager.main.reduceTheNumberOfLaserShots();
                isLaserActive = true;
                laser.enabled = true;
                laserCharge.SetActive(true);
                laserSound.Play();
                StartCoroutine(ShootLaser());
            }
        }
    }

    private void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(transform.up * bulletForce, ForceMode2D.Impulse);
        bulletSound.PlayOneShot(bulletSound.clip);
    }

    IEnumerator ShootLaser()
    {
        float time = 0;
        while (time <= laserTime)
        {
            laser.SetPosition(0, transform.position);

            Vector2 dir = transform.parent.position + (transform.up * 20);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
            if (hit.collider)
            {
                laser.SetPosition(1, hit.point);
                IHitHandling hitHandling = hit.collider.gameObject.GetComponent<IHitHandling>();
                if (hitHandling != null)
                {
                    hitHandling.laserHit();
                }
            }
            else
            {
                laser.SetPosition(1, dir);
            }
            time += Time.smoothDeltaTime;
            yield return null;
        }
        DisableLaser();    
    }

    private void DisableLaser()
    {
        isLaserActive = false;
        laser.SetPositions(new Vector3[]
        {
            Vector3.zero,
            Vector3.zero
        });
        laserSound.Stop();
        laser.enabled = false;
        laserCharge.SetActive(false);
    }

    private void OnDisable()
    {
        DisableLaser();
        StopCoroutine(ShootLaser());
    }
}
