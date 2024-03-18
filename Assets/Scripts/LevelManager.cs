using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Should be filled")]
    public GameObject levelBlockPrefab;
    public Transform LevelsLayout;
    [SerializeField] World world;
    public MessageWindow messageWindow;

    [Header("Public Field Variables")]
    public int currentLevel;
    public int maxLevel;
    public float creationDelay = .2f;
   

    private const string MaxLevel = "MaxLevel";
    private const string CurrentLevel = "CurrentLevel";

    void Start()
    {
        maxLevel = PlayerPrefs.GetInt(MaxLevel,0);
        currentLevel = PlayerPrefs.GetInt(CurrentLevel,0);
        StartCoroutine(BringMenu());
    }

    IEnumerator BringMenu(){
        yield return StartCoroutine(messageWindow.GetComponent<RectXformMover>().MoveOn());
        yield return StartCoroutine(CreateLevelBlocksCoroutine());
    }

    IEnumerator CreateLevelBlocksCoroutine()
    {
        for(int i = 0; i < world.levels.Length ; i++){
            GameObject levelBlockObj = Instantiate(levelBlockPrefab, LevelsLayout);
            LevelBlock levelBlock = levelBlockObj.GetComponent<LevelBlock>();
            levelBlock.level = i+1;
            levelBlock.Init();
            yield return new WaitForSecondsRealtime(creationDelay);
        }
    }
}
