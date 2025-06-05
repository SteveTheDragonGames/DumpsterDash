using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Flea_Emitter : MonoBehaviour
{
    public GameObject fleaPrefab;

    void Start()
    {
        float fleaSpawnTime = Random.Range(4f, 10f);
        InvokeRepeating(nameof(EmitFlea), fleaSpawnTime, fleaSpawnTime);
    }

    void EmitFlea()
    {
        Vector3 spawnPoint = transform.position + new Vector3(0, 0.5f, 0);
        GameObject spawnedFlea = Instantiate(fleaPrefab, spawnPoint, Quaternion.identity);

        float xForce = Random.Range(-2f, 2f);
        float yForce = Random.Range(2f, 5f);

        // Flip the flea sprite if hopping left
        if (xForce < 0)
        {
            spawnedFlea.GetComponent<SpriteRenderer>().flipX = true;
        }

        Rigidbody2D rb = spawnedFlea.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogWarning("Flea prefab is missing a Rigidbody component!");
        }
    }
}


