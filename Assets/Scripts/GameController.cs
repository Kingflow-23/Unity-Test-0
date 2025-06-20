using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public GameObject spaceshipPrefab;
    public GameObject[] LifeIcons;
    private GameObject gameOverSign;
    private GameObject LevelCompleteSign;
    public GameObject spaceship;

    private int numLivesLeft;
    private float respawnTime = 3.0f;
    private int maxLives = 4;

    public int maxAsteroids = 1;
    private int numAsteroids;

    private int myscore = 0;
    private Score scoreText;

    float minDistance = 2.0f;
    public float timeDied;
    private float finishTime;

    private bool gameFinished = false;

    int maxAttempts = 100;
    private void Awake()
    {
        numLivesLeft = maxLives;
        gameObject.name = "GameController";
        gameOverSign = GameObject.Find("GameOver");
        LevelCompleteSign = GameObject.Find("LevelCleared");

        myscore = 0;
        scoreText = GameObject.Find("Score").GetComponent<Score>();
        scoreText.UpdateScore(myscore);

        InitializeLevel();
    }


    private void InitializeLevel()
    {
        numAsteroids = maxAsteroids;
        for (int i = 0; i < numAsteroids; i++)
        {
            SpawnAsteroid();
        }
        spawnSpaceship();
        gameOverSign.SetActive(false);
        LevelCompleteSign.SetActive(false);
        gameFinished = false;
    }

    private void SpawnAsteroid()
    {
        bool valid;
        GameObject newAsteroid;
        int attempts = 0;

        do
        {
            newAsteroid = Instantiate(asteroidPrefab);
            newAsteroid.GetComponent<Asteroid>().setGameController(this);
            valid = CheckTooClose(newAsteroid);
            attempts++;
            if (!valid)
            {
                Debug.Log("Asteroid spawn attempt " + attempts + " failed.");
            }
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Max attempts reached for spawning asteroid. Giving up on this asteroid.");
                break;
            }
        } while (!valid);
    }

    public void AddAsteroids(int amount)
    {
        numAsteroids += amount;
    }

    
    public void RemoveAsteroid()
    {
        numAsteroids--;
    }

    // Spawn Spaceship
    private void spawnSpaceship()
    {
        bool valid = false;
        spaceship = null;
        int attempts = 0;

        do
        {
            spaceship = Instantiate(spaceshipPrefab);
            valid = CheckTooClose(spaceship);
            attempts++;
            if (!valid)
            {
                Debug.Log("Spaceship spawn attempt " + attempts + " failed.");
            }
        } while (!valid && attempts < maxAttempts);

        spaceship.GetComponent<Spaceship>().setGameController(this);
        numLivesLeft--;

        if (!valid)
        {
            Debug.LogWarning("Max attempts reached for spaceship. Trying fallback positions.");
            Vector3[] fallbackPositions = new Vector3[]
            {
                Vector3.zero,
                new Vector3(0, 2, 0),
                new Vector3(0, -2, 0),
                new Vector3(2, 0, 0),
                new Vector3(-2, 0, 0)
            };

            foreach (Vector3 pos in fallbackPositions)
            {
                if (IsSafePosition(pos))
                {
                    spaceship = Instantiate(spaceshipPrefab);
                    spaceship.transform.position = pos;
                    valid = true;
                    Debug.Log("Spaceship spawned at fallback position: " + pos);
                    break;
                }
            }
        }

        if (!valid)
        {
            Debug.LogWarning("No safe fallback found. Forcing spaceship spawn at center.");
            spaceship = Instantiate(spaceshipPrefab);
            spaceship.transform.position = Vector3.zero;
        }
    }

    public void increaseScore()
    {
        myscore += 30; // Increase the score by the given amount
        scoreText.UpdateScore(myscore); // Update the score display
    }

    private bool CheckTooClose(GameObject newAsteroid)
    {
        foreach (GameObject asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            if (asteroid == newAsteroid)
                continue;
            if (Vector3.Distance(newAsteroid.transform.position, asteroid.transform.position) < minDistance)
            {
                Destroy(newAsteroid);
                return false;
            }
        }
        return true;
    }

    private bool IsSafePosition(Vector3 position)
    {
        foreach (GameObject asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            if (Vector3.Distance(position, asteroid.transform.position) < minDistance)
                return false;
        }
        return true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        // Respawn spaceship if needed (after respawnTime has passed)
        if (spaceship == null && Time.time - timeDied >= respawnTime)
        {
            if (numLivesLeft > 0)
            {
                spawnSpaceship();
                Destroy(LifeIcons[numLivesLeft].gameObject);
            }
            else
            {
                gameOverSign.SetActive(true);
            }
        }

        if (numAsteroids == 0)
        {
            if (!gameFinished)
            {
                gameFinished = true;
                finishTime = Time.time;
            }
            else if (Time.time - finishTime >= respawnTime)
            {
                LevelCompleteSign.SetActive(true);
                StartCoroutine(Pause());
                gameFinished = false;
            }
        }
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(3f);

        if (maxAsteroids < 16)
            maxAsteroids *= 2;

        numAsteroids = maxAsteroids;

        Destroy(spaceship);
        spaceship = null;
        numLivesLeft++;
        InitializeLevel();
    }
}