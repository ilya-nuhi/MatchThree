using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    public GameObject collectionGoalLayout;

    public int collectionGoalBaseWidth = 125;

    CollectionGoalPanel[] m_collectionGoalPanels;

    // reference to graphic that fades in and out
    public ScreenFader screenFader;

    // UI.Text that stores the level name
    public TextMeshProUGUI levelNameText;

    // UI.Text that shows how many moves are left
    public TextMeshProUGUI movesLeftText;

    // reference to three-star score meter
    public ScoreMeter scoreMeter;

    // reference to the custom UI window
    public MessageWindow messageWindow;

    public GameObject movesCounter;

    public Timer timer;


    public override void Awake()
    {
        base.Awake();

        if (messageWindow != null)
        {
            messageWindow.gameObject.SetActive(true);
        }

        if (screenFader != null)
        {
            screenFader.gameObject.SetActive(true);
        }
    }

    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals)
    {
        if (collectionGoalLayout != null && collectionGoals != null && collectionGoals.Length != 0)
        {
            RectTransform rectXform = collectionGoalLayout.GetComponent<RectTransform>();
            rectXform.sizeDelta = new Vector2(collectionGoals.Length * collectionGoalBaseWidth, rectXform.sizeDelta.y);

            m_collectionGoalPanels = collectionGoalLayout.GetComponentsInChildren<CollectionGoalPanel>();

            for (int i = 0; i < m_collectionGoalPanels.Length; i++)
            {
                if (i < collectionGoals.Length && collectionGoals[i] != null)
                {
                    m_collectionGoalPanels[i].gameObject.SetActive(true);
                    m_collectionGoalPanels[i].collectionGoal = collectionGoals[i];
                    m_collectionGoalPanels[i].SetupPanel();
                }
                else
                {
                    m_collectionGoalPanels[i].gameObject.SetActive(false);
                }
            }


        }
    }

    public void UpdateCollectionGoalLayout()
    {
        foreach (CollectionGoalPanel panel in m_collectionGoalPanels)
        {
            if (panel != null && panel.isActiveAndEnabled)
            {
                panel.UpdatePanel();
            }
        }
    }

    public void EnableTimer(bool state)
    {
        if (timer != null)
        {
            timer.gameObject.SetActive(state);
        }
    }

    public void EnableMovesCounter(bool state)
    {
        if (movesCounter != null)
        {
            movesCounter.SetActive(state);
        }
    }

    public void EnableCollectionGoalLayout(bool state)
    {
        if (collectionGoalLayout != null)
        {
            collectionGoalLayout.SetActive(state);
        }
    }

}
