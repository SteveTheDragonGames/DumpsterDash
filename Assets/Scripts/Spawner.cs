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


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRats());
    }

    IEnumerator SpawnRats()
    {
        while(true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1,6));

            randomIndex = UnityEngine.Random.Range(0, rats.Length);
            randomSide = UnityEngine.Random.Range(0, 2);

            spawnedRat = Instantiate(rats[randomIndex]);

            if(randomSide ==0)
            {
                spawnedRat.transform.position = leftPos.position;
                spawnedRat.GetComponent<Rat>().speed = UnityEngine.Random.Range(3, 10);
            }
            else
            {
                spawnedRat.transform.localScale = new Vector3(-1f, 1f, 1f);//flip
                spawnedRat.transform.position = rightPos.position;
                spawnedRat.GetComponent<Rat>().speed = -UnityEngine.Random.Range(3, 10);
                
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
