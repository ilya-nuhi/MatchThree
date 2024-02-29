using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

// the GameManager is the master controller for the GamePlay

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{

    // reference to graphic that fades in and out
    public ScreenFader screenFader;

    // UI.Text that stores the level name
    public TextMeshProUGUI levelNameText;

    // UI.Text that shows how many moves are left
    public TextMeshProUGUI movesLeftText;

    // reference to the Board
    Board m_board;

    // is the player read to play?
    bool m_isReadyToBegin = false;

    // is the game over?
    bool m_isGameOver = false;

    public bool IsGameOver
    {
        get {
            return m_isGameOver;
        }
        set
        {
            m_isGameOver = value;
        }
    }

    // do we have a winner?
    bool m_isWinner = false;

    // are we ready to load/reload a new level?
    bool m_isReadyToReload = false;

    // reference to the custom UI window
    public MessageWindow messageWindow;

    // sprite for losers
    public Sprite loseIcon;

    // sprite for winners
    public Sprite winIcon;

    // sprite for the level goal
    public Sprite goalIcon;

    LevelGoal m_levelGoal;
    public ScoreMeter scoreMeter;
    LevelGoalTimed m_levelGoalTimed;

    public LevelGoalTimed LevelGoalTimed { get { return m_levelGoalTimed;}}

    public override void Awake()
    {
        base.Awake();

        m_levelGoal = GetComponent<LevelGoal>();
        m_levelGoalTimed = GetComponent<LevelGoalTimed>();

        // cache a reference to the Board
        m_board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();

    }

    void Start()
    {
        // position ScoreStar horizontally
        if (scoreMeter != null)
        {
            scoreMeter.SetupStars(m_levelGoal);
        }
        // get a reference to the current Scene
        Scene scene = SceneManager.GetActiveScene();

        // use the Scene name as the Level name
        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }

        // update the moves left UI
        m_levelGoal.movesLeft++;
        UpdateMoves();

        // start the main game loop
        StartCoroutine("ExecuteGameLoop");
    }

    // update the Text component that shows our moves left
    public void UpdateMoves()
    {
        // if the LevelGoal is not timed (e.g. LevelGoalScored)...
        if (m_levelGoalTimed == null)
        {
            // decrement a move
            m_levelGoal.movesLeft--;

            // update the UI
            if (movesLeftText != null)
            {
                movesLeftText.text = m_levelGoal.movesLeft.ToString();
            }
        }
        // if the LevelGoal IS timed...
        else
        {
            // change the text to read Infinity symbol
            if (movesLeftText != null)
            {
                movesLeftText.text = "\u221E";
                movesLeftText.fontSize = 70;
            }

        }
    }

    // this is the main coroutine for the Game, that determines are basic beginning/middle/end

    // each stage of the game must complete before we advance to the next stage
    // add as many stages here as necessary

    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");

        // wait for board to refill
        yield return StartCoroutine("WaitForBoardRoutine", 0.5f);

        yield return StartCoroutine("EndGameRoutine");
    }

    // switches ready to begin status to true
    public void BeginGame()
    {
        m_isReadyToBegin = true;

    }

    // coroutine for the level introduction
    IEnumerator StartGameRoutine()
    {
        // show the message window with the level goal
        if (messageWindow != null)
        {
            messageWindow.GetComponent<RectXformMover>().MoveOn();
            messageWindow.ShowMessage(goalIcon, "score goal\n" + m_levelGoal.scoreGoals[0].ToString(), "start");
        }

        // wait until the player is ready
        while (!m_isReadyToBegin)
        {
            yield return null;
        }

        // fade off the ScreenFader
        if (screenFader != null)
        {
            screenFader.FadeOff();
        }

        // wait half a second
        yield return new WaitForSeconds(0.5f);

        // setup the Board
        if (m_board != null)
        {
            m_board.SetupBoard();
        }
    }

    // coroutine for game play
    IEnumerator PlayGameRoutine()
    {
        // if level is timed, start the timer
        if (m_levelGoalTimed != null)
        {
            m_levelGoalTimed.StartCountdown();
        }
        // while the end game condition is not true, we keep playing
        // just keep waiting one frame and checking for game conditions
        while (!m_isGameOver)
        {

            m_isGameOver = m_levelGoal.IsGameOver();

            m_isWinner = m_levelGoal.IsWinner();

            // wait one frame
            yield return null;
        }
    }

    IEnumerator WaitForBoardRoutine(float delay = 0f)
    {
        if(m_levelGoalTimed != null){
            if(m_levelGoalTimed.timer != null){
                m_levelGoalTimed.timer.FadeOff();
                m_levelGoalTimed.timer.paused = true;
            }
        }
        if (m_board != null)
        {
            // this accounts for the swapTime delay in the Board's SwitchTilesRoutine BEFORE ClearAndRefillRoutine is invoked
            yield return new WaitForSeconds(m_board.swapTime);

            // wait while the Board is refilling
            while (m_board.isRefilling)
            {
                yield return null;
            }
        }

        // extra delay before we go to the EndGameRoutine
        yield return new WaitForSeconds(delay);
    }

    // coroutine for the end of the level
    IEnumerator EndGameRoutine()
    {
        // set ready to reload to false to give the player time to read the screen
        m_isReadyToReload = false;


        // if player beat the level goals, show the win screen and play the win sound
        if (m_isWinner)
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(winIcon, "YOU WIN!", "OK");
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWinSound();
            }
        } 
        // otherwise, show the lose screen and play the lose sound
		else
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(loseIcon, "YOU LOSE!", "OK");
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayLoseSound();
            }
        }
        // wait one second
        yield return new WaitForSeconds(1f);

        // fade the screen 
        if (screenFader != null)
        {
            screenFader.FadeOn();
        }

        // wait until read to reload
        while (!m_isReadyToReload)
        {
            yield return null;
        }

        // reload the scene (you would customize this to go back to the menu or go to the next level
        // but we just reload the same scene in this demo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		
    }

    // use this to acknowledge that the player is ready to reload
    public void ReloadScene()
    {
        m_isReadyToReload = true;
    }

    // score points and play a sound
    public void ScorePoints(GamePiece piece, int multiplier = 1, int bonus = 0)
    {
        if (piece != null)
        {
            if (ScoreManager.Instance != null)
            {
                // score points
                ScoreManager.Instance.AddScore(piece.scoreValue * multiplier + bonus);

                // update the scoreStars in the Level Goal component
                m_levelGoal.UpdateScoreStars(ScoreManager.Instance.CurrentScore);

                if (scoreMeter != null)
                {
                    scoreMeter.UpdateScoreMeter(ScoreManager.Instance.CurrentScore, m_levelGoal.scoreStars);
                }
            }

            // play scoring sound clip
            if (SoundManager.Instance != null && piece.clearSound !=null)
            {
                SoundManager.Instance.PlayClipAtPoint(piece.clearSound, Vector3.zero, SoundManager.Instance.fxVolume);
            }
        }
    }

    public void AddTime(int timeValue)
    {
        if (m_levelGoalTimed != null)
        {
            m_levelGoalTimed.AddTime(timeValue);
        }
    }





}
