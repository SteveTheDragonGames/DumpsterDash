
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour

{
    [SerializeField] private GameObject[] rats;

    private GameObject spawnedRat;
    private int randomIndex;
    private int randomSide;
    private float border;
 

    [SerializeField] private bool canSpawn = true;

    public float spawnBuffer = 5f;


    void Start()
    {
        border = GameManager.Instance.maxBorderLimitX + spawnBuffer;
        StartCoroutine(SpawnRats());
    }

    IEnumerator SpawnRats()
    {
        while (canSpawn)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1, 6));

            randomIndex = UnityEngine.Random.Range(0, rats.Length);
            randomSide = UnityEngine.Random.Range(0, 2);

            spawnedRat = Instantiate(rats[randomIndex]);
            RatAI ratScript = spawnedRat.GetComponent<RatAI>();
            ratScript.ResetRat();

            if (randomSide == 0)
            {
                spawnedRat.transform.position = new Vector3(-border, transform.position.y, transform.position.z);
                ratScript.moveSpeed = -UnityEngine.Random.Range(3, 10);
            }
            else
            {
                spawnedRat.transform.localScale = new Vector3(-1f, 1f, 1f);
                spawnedRat.transform.position = new Vector3(border, transform.position.y, transform.position.z);

                ratScript.moveSpeed = UnityEngine.Random.Range(3, 10);
            }
        }
    }
}
