using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ErrorHandler))]
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

    [Header("GUI")]
    //Score Variables
    [SerializeField]
    TextMeshProUGUI highScoreText;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI comboText;

    int highscore, score, combo, firstMultiplier = 4, secondMultiplier = 8, thirdMutliplier = 12;
    float comboMult =1;

    //UI Gameobjects - enable and disable based on state
    [SerializeField]
    UIScreen[] menus;

    [Header("Game State")]

    [SerializeField]
    GameState currentState;

    //Toggles for which matching methods to use in the game
    bool colourMatching, emotionMatching;

    [Header("Persistant Objects")]

    [SerializeField]
    Transform player;

    [SerializeField]
    Spawner spawner;

    string currentSong;

    bool cameraActive = false, pauseTrigger = false; // For when a seperate thread pauses the game
    ErrorHandler errorHandler;

    [System.Serializable]
    struct UIScreen
    {
        public string name;
        public GameObject gui;
    }

    //Random number generator for deterministic values (used for songs matching sequences)
    PseudoRandomNumberGenerator songSequenceGenerator;

    public enum GameState
    {
        Playing,
        PauseMenu,
        MainMenu,
        EndOfLevel,
        Settings,
        Controls,
        ChooseSong
    }


    // Start is called before the first frame update
    void Start()
    {
        //Assign error handler to manage error cases.
        errorHandler = GetComponent<ErrorHandler>();
        currentState = GameState.ChooseSong;
        StartLevel();
        //SetupSong("Rock");
    }

    // Update is called once per frame
    void Update()
    {
        if(pauseTrigger)
        {
            PauseGame(true);
            //reset trigger
            pauseTrigger = false;
        }

        //This acts as a finite state machine for the players state in the game
        switch (currentState)
        {      
            //Player interacts with the game to achieve a high score whilst the song is playing
            case GameState.Playing:
                SetUIActiveOnly("HUD");

                //Check if camera active - if disconnected pause game.
                if (!IsCameraActive())
                    PauseGame(true);
                break;
            //An interface that stops game time and allows the player to go back to main menu or resume
            case GameState.PauseMenu:
                SetUIActiveOnly("Pause Menu");
                break;
            //An interface where the player can navigate to other states
            case GameState.MainMenu:
                SetUIActiveOnly("Main Menu");
                break;
            //When the song is complete the player's score is highlighted and given a ranking
            case GameState.EndOfLevel:
                SetUIActiveOnly("Results");
                break;
            //An interface that instructs the player how to play the game
            case GameState.Controls:
                SetUIActiveOnly("HowToPlay");
                break;
            //Player can change sound levels with AR marker detection
            case GameState.Settings:
                SetUIActiveOnly("Settings");
                break;
            //Player chooses a song to play the game
            case GameState.ChooseSong:
                SetUIActiveOnly("Song Menu");
                break;
        }
    }


    //Sets Pause trigger to True. For Seperate Threads to pause game
    public void TriggerPause()
    {
        pauseTrigger = true;
    }

    //Set game to paused or play mode
    public void PauseGame(bool paused)
    {
        if (!(currentState == GameState.Playing || currentState == GameState.PauseMenu))
        {
            Debug.LogWarning("Cannot pause/unpause game whilst not playing the game.");
            return;
        }

        if (paused)
        {
            Time.timeScale = 0;
            currentState = GameState.PauseMenu;
            SoundManager.instance.PauseSong();
        }
        else
        {
            Time.timeScale = 1;
            currentState = GameState.Playing;
            SoundManager.instance.UnPauseSong();
        }
    }

    //Checks if there is an active camera
    bool IsCameraActive()
    {
        if (!cameraActive)
        {
            Debug.LogError("No Active Camera!");
            SoundManager.instance.PlaySoundEffect("Incorrect");
            errorHandler.IndicateError("no camera detected. connect camera to continue.");
            return false;
        }
        return true;

    }

    public void TurnColourMatchingOn(bool on)
    {
        //ensure at least one matching method is being used
        if (!emotionMatching && !on)
        {
            emotionMatching = true;
            errorHandler.IndicateError("At least one matching method must be used!");
        }
        colourMatching = on;
    }

    public void TurnEmotionMatchingOn(bool on)
    {
        //ensure at least one matching method is being used
        if (!colourMatching && !on)
        {
            colourMatching = true;
            errorHandler.IndicateError("At least one matching method must be used!");
        }
        emotionMatching = on;
    }

    //Returns True if game is in a pausable state
    public bool CanPause()
    {
        if (currentState == GameState.Playing)
            return true;
        return false;
    }

    //Sets the level up to play a song
    public void SetupLevel(string song)
    {
        //Check if camera input is available
        if(!IsCameraActive())
            return;

        currentState = GameState.Playing;
        SoundManager.instance.PlaySoundEffect("Correct");
        currentSong = song;
        songSequenceGenerator = PseudoRandomNumberGenerator.instance;
        songSequenceGenerator.ResetSequence();
        MatchSong matchSong = SoundManager.instance.GetMatchSong(currentSong);
        spawner.SetSpawnerForSong(matchSong.objectSpeed, matchSong.beat);
        spawner.SetSpawnerActive(true);
        //delay song
        StartCoroutine(SongIntro());
    }

    public void GoToSongSelection()
    {

    }

    //start a song with a delay to match the input and countdown to song starting
    IEnumerator SongIntro()
    {
        if (spawner != null && SoundManager.instance!=null)
        {
            float delay = spawner.GetSongDelay() + SoundManager.instance.GetMatchSong(currentSong).songDelay;
            SoundManager.instance.PlaySoundEffect("Intro");
            //Countdown
            yield return new WaitForSeconds(delay / 4);
            SoundManager.instance.PlaySoundEffect("Countdown");
            yield return new WaitForSeconds(delay / 4);
            SoundManager.instance.PlaySoundEffect("Countdown");
            yield return new WaitForSeconds(delay / 4);
            SoundManager.instance.PlaySoundEffect("Countdown");
            yield return new WaitForSeconds(delay / 4);
            SoundManager.instance.PlaySoundEffect("CountdownOver");

            //Play song after delay to sync with match objects
            SoundManager.instance.PlaySong(currentSong);       
            //Stop match objects spawning when song stops
            yield return new WaitForSeconds(SoundManager.instance.GetSongLength(currentSong)- (spawner.GetSongDelay()));
            spawner.SetSpawnerActive(false);

        }
        else
        {
            Debug.LogError(spawner + " has not been assigned!");
            yield return null;
        }
    }

    //Resets player position to the start of the level
    void ResetToStartPosition()
    {

    }

    void SetUIActiveOnly(string name)
    {
        foreach(UIScreen screen in menus)
        {
            if (screen.name == name)
                screen.gui.SetActive(true);
            else
                screen.gui.SetActive(false);
        }
    }

    //Spawns an object much the player must match up with the correct input
    void SpawnMatchObject()
    {

    }

    //Tick function - 
    void PlayLevel()
    {
        
    }

    public void SetCameraActivity(bool activity)
    {
        cameraActive = activity;
    }

    //Starts level and resets HUD 
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

    //Increases Score and updates HUD
    public void IncreaseScore(int increase)
    {
        score += Mathf.RoundToInt(increase * comboMult);
        scoreText.text = score.ToString();
        if (score >= highscore)
            highScoreText.text = score.ToString();
        IncreaseCombo();
    }

    //Resets combo on HUD
    public void ResetCombo()
    {
        combo = 0;
        comboText.text = combo.ToString();
    }

    //Reset score on HUD 
    void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    //Sets highscore on HUD 
    void SetHighScore()
    {
        highScoreText.text = score.ToString();
    }

    //Increments Combo and updates HUD
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
