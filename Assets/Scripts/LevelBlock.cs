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
    public Image m_image;
    public TextMeshProUGUI levelText;
    public bool state = false;

    private void Awake() {
        m_image = GetComponentInChildren<Image>();
    }
    private void Start() {
        m_image.color = Color.red;
        levelText.color = Color.green;
        gameObject.GetComponent<Scaler>().ScaleUp();
    }

    public void Init(){
        levelText.text = level.ToString();
    }

    public void SelectLevel(){
        m_image.color = Color.white;
        levelText.color = Color.black;
        state = true;
        UnSelectOtherLevels();

    }

    void UnSelectOtherLevels(){
        LevelBlock[] levelBlocks = Object.FindObjectsOfType<LevelBlock>();
        foreach(var levelblock in levelBlocks){
            if(levelblock.state == true){
                levelblock.m_image.color = Color.gray;
                levelblock.levelText.color = Color.white;
                levelblock.state = false;
            }
        }
    }

}
