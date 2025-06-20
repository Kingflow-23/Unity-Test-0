using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;
    private int health = 1;
    private float maxSpeed = 2f;
    private float maxX = 10.15f;
    private float maxY = 6.2f;
    private int scale;
    private int maxScale = 1;
    public float childAsteroidOffset = 1f;
    public GameObject asteroidPrefab;
    public GameObject explosionPrefab;
    private GameController gameController;
    private void Awake()
    {
        scale = maxScale;
        rb = GetComponent<Rigidbody2D>();
        gameObject.tag = "Asteroid";
        gameObject.name = "Asteroid";
        transform.position = new Vector3(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY), 0);
        rb.linearVelocity = Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(Random.Range(0.5f, maxSpeed), 0, 0);
    }

    public void setGameController(GameController _gameController)
    {
        gameController = _gameController;
    }

    void Update()
    {
        if (transform.position.x > maxX)
            transform.position = new Vector3(-maxX, transform.position.y, 0);
        if (transform.position.x < -maxX)
            transform.position = new Vector3(maxX, transform.position.y, 0);
        if (transform.position.y > maxY)
            transform.position = new Vector3(transform.position.x, -maxY, 0);
        if (transform.position.y < -maxY)
            transform.position = new Vector3(transform.position.x, maxY, 0);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    private void Die()
    {
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = transform.position;

        int scaleFactor = maxScale - scale;
        AsteroidExplosion explosionAudio = explosion.GetComponent<AsteroidExplosion>();
        if (explosionAudio != null)
            explosionAudio.SetAudio(0.8f - scaleFactor * 0.25f, 1f + scaleFactor);
        else
            Debug.LogWarning("AsteroidExplosion component is missing on the explosion prefab.");

        ParticleSystem partSys = explosion.GetComponent<ParticleSystem>();
        partSys.Stop();
        var main = partSys.main;
        if (scale <= 3 && scale > 0)
            main.startSize = scale;
        else if (scale == 0)
            main.startSize = 0.5f;
        else
            main.startSize = 1f;
        main.simulationSpeed = Mathf.Lerp(1f, 3f, (float)(maxScale - scale) / maxScale);
        partSys.Play();

        if (scale > 0)
        {
            spawnChildAsteroids();
            gameController.AddAsteroids(4);
        }
        gameController.RemoveAsteroid();
        Destroy(gameObject);
    }

    private void spawnChildAsteroids()
    {
        Vector2[] newDirection = new Vector2[4]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1)
        };

        float randAngle = Random.Range(0, 360);
        for (int i = 0; i < 4; i++)
        {
            GameObject newAsteroid = Instantiate(asteroidPrefab);
            newAsteroid.GetComponent<Asteroid>().setGameController(gameController);
            Asteroid asteroidHandle = newAsteroid.GetComponent<Asteroid>();
            newDirection[i] = Quaternion.Euler(0, 0, randAngle + Random.Range(-30, 30)) * newDirection[i];
            newAsteroid.transform.position = transform.position + (Vector3)(newDirection[i] * childAsteroidOffset);
            newAsteroid.transform.localScale = transform.localScale / 2;
            asteroidHandle.scale = scale - 1;
            asteroidHandle.childAsteroidOffset = childAsteroidOffset / 2;
            Rigidbody2D childRb = newAsteroid.GetComponent<Rigidbody2D>();
            childRb.mass = rb.mass / 4;
            childRb.AddForce((Vector3)(newDirection[i] * childAsteroidOffset * childAsteroidOffset * 5));
        }
    }

    public void takeDamage()
    {
        health--;
        if (health == 0)
        {
            Die();
            gameController.increaseScore();
        }
    }
}
