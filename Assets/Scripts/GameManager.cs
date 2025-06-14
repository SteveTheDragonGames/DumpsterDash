using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Make this accessible everywhere.
    public static GameManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        //If no game manager, then let this one be it.
        if (Instance == null) Instance = this;
        else
        {
            //Another game manager exists, destroy this one.
            Destroy(gameObject);
            return;
        }

        //This object persists through all scenes.
        DontDestroyOnLoad(gameObject);

    }

    public void AwardPoints(int amount)
    {
        ScoreManager.instance.AddScore(amount);
    }
}
