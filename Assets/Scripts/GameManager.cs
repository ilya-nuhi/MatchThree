using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

// the GameManager is the master controller for the GamePlay

public class GameManager : Singleton<GameManager>
{

    // number of moves left before Game Over
    public int movesLeft = 30;

    // goal to reach to meet Game Win condition
    public int scoreGoal = 10000;

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


    void Start()
    {
        // cache a reference to the Board
        m_board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();

        // get a reference to the current Scene
        Scene scene = SceneManager.GetActiveScene();

        // use the Scene name as the Level name
        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }

        // update the moves left UI
        UpdateMoves();

        // start the main game loop
        StartCoroutine("ExecuteGameLoop");
    }

    // update the Text component that shows our moves left
    public void UpdateMoves()
    {
        if (movesLeftText != null)
        {
            movesLeftText.text = movesLeft.ToString();

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
            messageWindow.ShowMessage(goalIcon, "score goal\n" + scoreGoal.ToString(), "start");
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
        // while the end game condition is not true, we keep playing
        // just keep waiting one frame and checking for game conditions
        while (!m_isGameOver)
        {
            // if our current score is greater than the level goal, then we win and end the game
            if (ScoreManager.Instance != null)
            {
                if (ScoreManager.Instance.CurrentScore >= scoreGoal)
                {
                    m_isGameOver = true;
                    m_isWinner = true;
                }
            }

            // if we run out of moves, then we lose and end the game
            if (movesLeft == 0)
            {
                m_isGameOver = true;
                m_isWinner = false;
            }

            // wait one frame
            yield return null;
        }
    }

    IEnumerator WaitForBoardRoutine(float delay = 0f)
    {
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









}
