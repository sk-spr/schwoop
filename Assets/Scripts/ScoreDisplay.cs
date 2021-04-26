using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText;
    public Text highScoreText;
    private scoreKeeper keeper;
    void Start()
    {
        keeper = FindObjectOfType<scoreKeeper>() ?? Instantiate(new scoreKeeper());
        int highscore = getHighScore();
        scoreText.text = $"score: {(int)keeper.score}";
        if (keeper.score > highscore)
        {
            highscore = (int)keeper.score;
            saveHighScore(highscore);
        }

        highScoreText.text = $"high score: {highscore}";
    }
    private int getHighScore() => PlayerPrefs.GetInt("highscore");

    private void saveHighScore(int score) => PlayerPrefs.SetInt("highscore", score);

}
