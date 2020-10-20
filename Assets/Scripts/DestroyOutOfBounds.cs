using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{

    private Vector2 boundsSize;
    private float increaseBounds = 3f;

    void Start()
    {
        boundsSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        boundsSize.x += increaseBounds;
        boundsSize.y += increaseBounds;
    }

    void Update()
    {
        if (Mathf.Abs(transform.position.x) >= Mathf.Abs(boundsSize.x) ||
            Mathf.Abs(transform.position.y) >= Mathf.Abs(boundsSize.y))
        {
            Destroy(gameObject);
        }
    }
}
