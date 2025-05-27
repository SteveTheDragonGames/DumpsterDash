using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Text scoreText;
    public static ScoreManager instance;
    private int currentScore = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {

        scoreText = GetComponent<UnityEngine.UI.Text>();
        UpdateScoreText();
        
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.P)) AddScore(1);
    }

    public void AddScore(int amount)
    {
        if (currentScore + amount < 0) return;
        currentScore += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + currentScore.ToString();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();
    }
}


