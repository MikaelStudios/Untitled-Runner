using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    public float score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public int highScore;
   
    // Start is called before the first frame update
    void Start()
    {
      

    }

    // Update is called once per frame
    void Update()
    {
        
        ScoreCounter();

    }
    void ScoreCounter()
    {
       
        if (GameMaster.instance.gameStart)
        {
            score += Time.deltaTime * 1;
            highScore = (int)score;
            scoreText.text = highScore.ToString();
            if (PlayerPrefs.GetInt("score") <= highScore)
            {
                PlayerPrefs.SetInt("score", highScore);
            }
           
        }
        if(GameMaster.instance.countDown)
        {

            highScoreText.gameObject.SetActive(false);
        }


        highScoreText.text = "Best: " + PlayerPrefs.GetInt("score").ToString();
    }
  

}

