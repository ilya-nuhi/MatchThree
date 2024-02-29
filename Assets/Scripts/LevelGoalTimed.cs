using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalTimed : LevelGoal
{
 
    public Timer timer;
    int m_maxTime;

    void Start()
    {
        if (timer != null)
        {
            timer.InitTimer(timeLeft);
        }

        m_maxTime = timeLeft;
    }

    // public method to start the timer
    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    // decrement the timeLeft each second
    IEnumerator CountdownRoutine()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;

            if (timer != null)
            {
                timer.UpdateTimer(timeLeft);
            }
        }
    }

    // did we win?
    public override bool IsWinner()
    {
        // we scored higher than the lowest score goal, we win
        if (ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.CurrentScore >= scoreGoals[0]);
        }
        return false;
    }

    // is the game over?
    public override bool IsGameOver()
    {
        int maxScore = scoreGoals[scoreGoals.Length - 1];

        // if we score higher than the last score goal, end the game
        if (ScoreManager.Instance.CurrentScore >= maxScore)
        {
            return true;
        }

        // end the game if we have no moves left
        return (timeLeft <= 0);
    }

    public void AddTime(int timeValue)
    {
        timeLeft += timeValue;
        timeLeft = Mathf.Clamp(timeLeft, 0, m_maxTime);

        if (timer != null)
        {
            timer.UpdateTimer(timeLeft);
        }

    }
}
