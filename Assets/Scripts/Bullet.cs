using UnityEngine;

/// <summary>
/// Destroys the bullet when it leaves the screen bounds or collides with an asteroid.
/// </summary>
public class Bullet : MonoBehaviour
{
    private float maxX = 12f;
    private float maxY = 7f;
    private Rigidbody2D rb;

    void Update()
    {
        if (transform.position.x > maxX || transform.position.x < -maxX ||
            transform.position.y > maxY || transform.position.y < -maxY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            Asteroid asteroid = collision.GetComponent<Asteroid>();
            if (asteroid != null)
            {
                asteroid.takeDamage();
            }
            Destroy(gameObject);
        }
    }
}
