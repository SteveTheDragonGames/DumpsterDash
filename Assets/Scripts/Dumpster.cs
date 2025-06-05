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

    public GameObject Lid_Closed;
    public GameObject Lid_Open;

    void Start()
    {
        CloseLid();
    }


    public JunkItem GetRandomJunk()
    {
        int index = UnityEngine.Random.Range(0, level1Junk.Count);
        return level1Junk[index];
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



}//Class Dumpster
