using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public GameController gameController;
    public AudioSource audioSource;
    public AudioClip ShootingSoundFX;
    public AudioClip thrustersFX;

    private float turnSpeed = 100f;
    private Vector3 shipDirection = new Vector3(0, 1, 0);
    private Rigidbody2D rb;
    private float thrust = 0.00015f;
    private float bulletSpeed = 20f;
    private float maxX = 9.2f;
    private float maxY = 5.2f;
    private float maxSpeed = 2.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        gameObject.tag = "Spaceship";
        gameObject.name = "Spaceship";
        if (gameController == null)
            gameController = GameObject.Find("GameController")?.GetComponent<GameController>();
    }

    public void setGameController(GameController _gameController)
    {
        gameController = _gameController;
    }

    void Update()
    {
        float turnAngle;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turnAngle = turnSpeed * Time.deltaTime;
            transform.Rotate(0, 0, turnAngle);
            shipDirection = Quaternion.Euler(0, 0, turnAngle) * shipDirection;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            turnAngle = -turnSpeed * Time.deltaTime;
            transform.Rotate(0, 0, turnAngle);
            shipDirection = Quaternion.Euler(0, 0, turnAngle) * shipDirection;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(shipDirection * thrust);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            audioSource.clip = thrustersFX;
            audioSource.Play();
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            audioSource.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(ShootingSoundFX);
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = shipDirection * bulletSpeed;
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        if (transform.position.x > maxX)
            transform.position = new Vector3(-maxX, transform.position.y, 0);
        else if (transform.position.x < -maxX)
            transform.position = new Vector3(maxX, transform.position.y, 0);

        if (transform.position.y > maxY)
            transform.position = new Vector3(transform.position.x, -maxY, 0);
        else if (transform.position.y < -maxY)
            transform.position = new Vector3(transform.position.x, maxY, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            gameController.timeDied = Time.time;
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
