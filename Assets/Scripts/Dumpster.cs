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
    public GameObject racoon;
    public List<JunkItem> level1Junk;
    public List<JunkItem> Level2Junk;
    public List<JunkItem> Level3Junk;
    public List<JunkItem> Level4Junk;
    public List<JunkItem> Level5Junk;


    public GameObject Lid_Closed;
    public GameObject Lid_Open;

    void Start()
    {
        CloseLid();
    }


    public JunkItem GetRandomJunk(int currentLevel)
    {
        List<JunkItem> validJunk = new List<JunkItem>();

        if (currentLevel >= 1) validJunk.AddRange(level1Junk);
        if (currentLevel >= 3) validJunk.AddRange(Level2Junk);
        if (currentLevel >= 6) validJunk.AddRange(Level3Junk);
        if (currentLevel >= 9) validJunk.AddRange(Level4Junk);
        if (currentLevel >= 11) validJunk.AddRange(Level5Junk);

        if (validJunk.Count == 0)
        {
            Debug.LogWarning("No junk items found for this level!");
            return null;
        }

        int index = UnityEngine.Random.Range(0, validJunk.Count);
        return validJunk[index];
    }


    public void OpenLid()
    {
        Lid_Open.SetActive(true);
        Lid_Closed.SetActive(false);
    }

    public void CloseLid()
    {
        Lid_Open.SetActive(false);
        Lid_Closed.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        CloseLid();
    }



}//Class Dumpster
