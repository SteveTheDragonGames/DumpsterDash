using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SpawnRaccoon : MonoBehaviour
{


    void Start()
    {

        StartCoroutine(nameof(WaitASecond));

        Vector2 spawnPosition = this.transform.position;
        Vector2 playerPosition = GameObject.Find("Player").transform.position;

        //determine direction
        if (playerPosition.x < spawnPosition.x)
        {
            //player is to the right, face Right.   
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            //player is to the left, face Left.    
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    IEnumerator WaitASecond()
    {
        yield return new WaitForSeconds(1f);
        //that's it.
    }



}
