using UnityEngine;

public class Flea : MonoBehaviour
{
    public AudioClip boingSound; // Assign in inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = boingSound;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; // 2D sound
        //audioSource.volume = 0.3f; // 👈 Set volume here (0 = silent, 1 = full blast)
        audioSource.pitch = Random.Range(0.9f, 1.2f);

        audioSource.Play(); // Play the boing when flea spawns

        Destroy(gameObject, UnityEngine.Random.Range(1f,4f)); // Auto-destroy after 2 seconds
    }
}

