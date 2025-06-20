using System.Collections;
using UnityEngine;

public class AsteroidExplosion : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        gameObject.name = "AsteroidExplosion";
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    public void SetAudio(float volumeLevel, float pitchLevel)
    {
        audioSource.volume = volumeLevel;
        audioSource.pitch = pitchLevel;
        audioSource.Play();
    }
}
