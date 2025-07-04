using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Spawner : MonoBehaviour

{

    [SerializeField] private GameObject[] rats;
    [SerializeField] private Transform leftPos, rightPos;

    private GameObject spawnedRat;
    private int randomIndex;
    private int randomSide;


    void Start()
    {
        StartCoroutine(SpawnRats());
    }

    IEnumerator SpawnRats()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1, 6));

            randomIndex = UnityEngine.Random.Range(0, rats.Length);
            randomSide = UnityEngine.Random.Range(0, 2);

            spawnedRat = Instantiate(rats[randomIndex]);
            RatAI ratScript = spawnedRat.GetComponent<RatAI>();
            ratScript.ResetRat();

            if (randomSide == 0)
            {
                spawnedRat.transform.position = leftPos.position;
                ratScript.moveSpeed = -UnityEngine.Random.Range(3, 10);
            }
            else
            {
                spawnedRat.transform.localScale = new Vector3(-1f, 1f, 1f);
                spawnedRat.transform.position = rightPos.position;
                ratScript.moveSpeed = UnityEngine.Random.Range(3, 10);
            }
        }
    }
}
