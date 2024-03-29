using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scaler))]
public class LevelBlock : MonoBehaviour
{
    public bool isUnlocked = false;
    public int level;
    public bool isSelected = false;
    public bool isCurrentLevel = false;
    public bool isLocked = false;
    public Image buttonImage;
    public TextMeshProUGUI levelText;
    public bool state = false;
    public Button m_button;
    private const string CurrentLevel = "CurrentLevel";

    private void Awake() {
        m_button = GetComponentInChildren<Button>();
    }
    private void Start() {
        if(isCurrentLevel){
            buttonImage.color = Color.white;
            levelText.color = Color.black;
            state = true;
        }
        else if(isLocked){
            buttonImage.color = Color.black;
            levelText.color = Color.black;
            m_button.interactable = false;
        }
        else{
            buttonImage.color = Color.gray;
            levelText.color = Color.white;
        }
        
        gameObject.GetComponent<Scaler>().ScaleUp();
    }

    public void Init(){
        levelText.text = (level+1).ToString();
    }

    public void SelectLevel(){
        UnSelectOtherLevels();
        buttonImage.color = Color.white;
        levelText.color = Color.black;
        state = true;
        PlayerPrefs.SetInt(CurrentLevel, level);
    }

    void UnSelectOtherLevels(){
        LevelBlock[] levelBlocks = Object.FindObjectsOfType<LevelBlock>();
        foreach(var levelblock in levelBlocks){
            if(levelblock.state == true){
                levelblock.buttonImage.color = Color.gray;
                levelblock.levelText.color = Color.white;
                levelblock.state = false;
            }
        }
    }

}
