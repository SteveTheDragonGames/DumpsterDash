using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JunkItem
{
    public string name;
    public int scoreValue;
    public string description;
    public GameObject junkItem;
}// Class JunkItem

public class Dumpster : MonoBehaviour
{

    public List<JunkItem> level1Junk;


    public JunkItem GetRandomJunk()
    {
        int index = UnityEngine.Random.Range(0, level1Junk.Count);
        return level1Junk[index];
    }



}//Class Dumpster
