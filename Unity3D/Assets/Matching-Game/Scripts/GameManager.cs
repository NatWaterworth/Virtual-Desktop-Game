using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError("Cannot be more than 1 instance of a singleton object: " + this);
            Destroy(gameObject);
        }
    }

    #endregion

    //Score Variables
    [SerializeField]
    TextMeshProUGUI highScoreText, scoreText, comboText;
    int highscore, score, combo, firstMultiplier = 4, secondMultiplier = 8, thirdMutliplier = 12;
    float comboMult =1;

    [SerializeField]
    GameState currentState;

    [SerializeField]
    Transform player;

    [SerializeField]
    [Range(1, 3)]
    int difficulty;

    [SerializeField]
    float gameSpeed;

    public enum GameState
    {
        Playing,
        PauseMenu,
        MainMenu
    }


    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.Playing;
        StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.Playing:
                break;
            case GameState.PauseMenu:
                break;
            case GameState.MainMenu:
                break;
        }
    }

    //Resets player position to the start of the level
    void ResetToStartPosition()
    {

    }

    //Spawns an object much the player must match up with the correct input
    void SpawnMatchObject()
    {

    }

    //Tick function - 
    void PlayLevel()
    {
        
    }

    void StartLevel()
    {
        ResetCombo();
        ResetScore();
        SetHighScore();
    }

    //Return Transform of Player
    public Transform GetPlayerTransform()
    {
        if(player!=null)
            return player.transform;
        else
        {
            Debug.LogError("No player assigned in:" + this);
            return null;
        }
    }

    public void IncreaseScore(int increase)
    {
        score += Mathf.RoundToInt(increase * comboMult);
        scoreText.text = score.ToString();
        if (score >= highscore)
            highScoreText.text = score.ToString();
        IncreaseCombo();
    }

    public void ResetCombo()
    {
        combo = 0;
    }

    void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    void SetHighScore()
    {
        highScoreText.text = score.ToString();
    }

    void IncreaseCombo()
    {
        combo++;
        comboText.text = combo.ToString();


        if (combo == firstMultiplier)
        {
            //Play Sound
            FindObjectOfType<SoundManager>().PlaySoundEffect("Combo1");
            comboMult = 1.2f;
        }
        else if(combo == secondMultiplier)
        {
            //Play Sound
            FindObjectOfType<SoundManager>().PlaySoundEffect("Combo2");
            comboMult = 1.5f;
        }
        else if(combo == thirdMutliplier)
        {
            //Play Sound
            FindObjectOfType<SoundManager>().PlaySoundEffect("Combo3");
            comboMult = 2f;
        }
    }
}
