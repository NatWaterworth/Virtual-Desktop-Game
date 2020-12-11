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

    int colourMatchWeight = 4,emotionMatchWeight = 2;

    //UI Gameobjects - enable and disable based on state
    [SerializeField]
    UIScreen[] menus;

    [Space]
    [SerializeField]
    //Toggles for setting which matching methods to use in the game
    Toggle colourMatchingToggle, emotionMatchingToggle;

    [Header("Game State")]

    [SerializeField]
    GameState currentState, previousState;


    [Header("Persistant Objects")]

    [SerializeField]
    Transform player;

    [SerializeField]
    MatchArea informationPort;

    [SerializeField]
    Spawner spawner;

    string currentSong;
    [SerializeField]
    bool cameraActive = false, pauseTrigger = false, playerInput = true; // For when a seperate thread pauses the game
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
        SwitchState(GameState.MainMenu);
        ResetHUD();
        //SetupSong("Rock");
    }

    // Update is called once per frame
    void Update()
    {
        //Don't allow input to have an effect on gameplay when set to false
        if (!playerInput)
            return;

        //Checks if pause has been triggered by parallel thread
        if(pauseTrigger)
        {
            PauseGame(true);
            //reset trigger
            pauseTrigger = false;
        }

        //Go Back Button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackButton();
        }

        //This acts as a finite state machine for the players state in the game
        switch (currentState)
        {      
            //Player interacts with the game to achieve a high score whilst the song is playing
            case GameState.Playing:
                //Check if camera active - if disconnected pause game.
                if (!IsCameraActive() || !informationPort.IsOpenCVActive())
                    PauseGame(true);
                break;
            //An interface that stops game time and allows the player to go back to main menu or resume
            case GameState.PauseMenu:
                break;
            //An interface where the player can navigate to other states
            case GameState.MainMenu:
                break;
            //When the song is complete the player's score is highlighted and given a ranking
            case GameState.EndOfLevel:
                break;
            //An interface that instructs the player how to play the game
            case GameState.Controls:
                break;
            //Player can change sound levels with AR marker detection
            case GameState.Settings:
                break;
            //Player chooses a song to play the game
            case GameState.ChooseSong:
                break;
        }
    }

    //Takes player to the previous state or pauses the game in play mode.
    void BackButton()
    {
        if(currentState == GameState.PauseMenu)
        {
            PauseGame(false);
        }
        else if (currentState == GameState.Playing)
        {
            PauseGame(true);
        }
        else if (currentState == GameState.MainMenu)
        {
            //we don't want to go back from this point as its the root
            return;
        }
        else if (currentState == GameState.EndOfLevel)
        {
            //ensure back button takes you back to main menu
            SwitchState(GameState.MainMenu);
        }
        else
            SwitchState(previousState);
    }


    //Switches state
    void SwitchState(GameState state)
    {
        //Confirmation Sound
        SoundManager.instance.PlaySoundEffect("Click");

        //assign previous state
        previousState = currentState;
        //assign new state
        currentState = state;

        //apply appropriate UI to match state
        switch (currentState)
        {
            //Player interacts with the game to achieve a high score whilst the song is playing
            case GameState.Playing:
                SoundManager.instance.StopMusic("MenuMusic");
                SetUIActiveOnly("HUD");
                break;
            //An interface that stops game time and allows the player to go back to main menu or resume
            case GameState.PauseMenu:
                SetUIActiveOnly("Pause Menu");
                break;
            //An interface where the player can navigate to other states
            case GameState.MainMenu:
                SoundManager.instance.PlayMusic("MenuMusic");
                SetUIActiveOnly("Main Menu");
                break;
            //When the song is complete the player's score is highlighted and given a ranking
            case GameState.EndOfLevel:
                SoundManager.instance.PlayMusic("MenuMusic");
                SetUIActiveOnly("Results");
                break;
            //An interface that instructs the player how to play the game
            case GameState.Controls:
                SetUIActiveOnly("How To Play");
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

    //Restarts level erasing any current score and resetting to score to 0
    public void RestartLevel()
    {
        //reset score
        SetupLevel(currentSong);
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
            SwitchState(GameState.PauseMenu);
            SoundManager.instance.PauseSong();
        }
        else
        {
            Time.timeScale = 1;
            SwitchState(GameState.Playing);
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

    bool IsOpenCVConnected()
    {
        if(informationPort == null)
        {
            Debug.LogError("information port not set for game manager!");
            SoundManager.instance.PlaySoundEffect("Incorrect");
            errorHandler.IndicateError("information port not set for game manager!");
            return false;
        }
        if (!informationPort.IsOpenCVActive())
        {
            Debug.LogError("Python environment not active!");
            SoundManager.instance.PlaySoundEffect("Incorrect");
            errorHandler.IndicateError("opencv application is not running.");
            return false;
        }
        return true;
    }

    public void TurnColourMatchingOn(Toggle toggle)
    {
        //ensure at least one matching method is being used
        if (!emotionMatchingToggle.isOn && !toggle.isOn)
        {
            emotionMatchingToggle.isOn = true;
            errorHandler.IndicateError("at least one matching method must be used!");
        }
        colourMatchingToggle.isOn = toggle.isOn;
    }

    public void TurnEmotionMatchingOn(Toggle toggle)
    {
        //ensure at least one matching method is being used
        if (!colourMatchingToggle.isOn && !toggle.isOn)
        {
            colourMatchingToggle.isOn = true;
            errorHandler.IndicateError("at least one matching method must be used!");
        }
        emotionMatchingToggle.isOn = toggle.isOn;
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
        //Check if openCV-Python has connected by detecting inputs from port
        if (!IsOpenCVConnected())
            return;

        //Check if camera input is available
        if (!IsCameraActive())
            return;

        //Reset UI to  values
        ResetHUD();

        //Set current state
        SwitchState(GameState.Playing);

        //Ensure game is not paused
        PauseGame(false);

      
        SoundManager.instance.PlaySoundEffect("Correct");
        currentSong = song;
        songSequenceGenerator = PseudoRandomNumberGenerator.instance;
        songSequenceGenerator.ResetSequence();
        MatchSong matchSong = SoundManager.instance.GetMatchSong(currentSong);
        spawner.SetSpawnerForSong(matchSong.objectSpeed, matchSong.beat);

        //Set spawners spawn weights
        if (emotionMatchingToggle.isOn)
            spawner.SetEmotionSpawnerRate(emotionMatchWeight);
        else
            spawner.SetEmotionSpawnerRate(0);

        if (colourMatchingToggle.isOn)
            spawner.SetColourSpawnerRate(emotionMatchWeight);
        else
            spawner.SetColourSpawnerRate(0);

        //Reset spawner
        spawner.ResetSpawner();
        //Set spawner to active
        spawner.SetSpawnerActive(true);
        //delay song
        StartCoroutine(SongIntro());
    }

    public void GoToSongSelection()
    {
        SwitchState(GameState.ChooseSong);
    }

    //start a song with a delay to match the input and countdown to song starting
    IEnumerator SongIntro()
    {
        
        //ensure song is not already playing.
        SoundManager.instance.StopSong();

        if (spawner != null && SoundManager.instance!=null)
        {
            //stop player from pausing during countdown sounds remained synced
            playerInput = false;

            float delay = spawner.GetSongDelay() + SoundManager.instance.GetMatchSong(currentSong).songDelay;
            SoundManager.instance.PlaySoundEffect("Intro");
            //Countdown
            Debug.Log("Play Intro!");
            yield return new WaitForSeconds(delay / 4);
            for(int i =0; i < 3; i++)
            {
                Debug.Log("Play Countdown!");
                SoundManager.instance.PlaySoundEffect("Countdown");
                yield return new WaitForSeconds(delay / 4);
            }
            Debug.Log("Countdown Over!");
            SoundManager.instance.PlaySoundEffect("CountdownOver");

            //allow player input again
            playerInput = true;

            //Play song after delay to sync with match objects
            SoundManager.instance.PlaySong(currentSong);       
            //Stop match objects spawning when song stops
            yield return new WaitForSeconds(SoundManager.instance.GetSongLength(currentSong)- (spawner.GetSongDelay()));
            spawner.SetSpawnerActive(false);

            yield return new WaitForSeconds(spawner.GetSongDelay());
            SoundManager.instance.PlaySoundEffect("Victory");
            //Small delay before end of level
            yield return new WaitForSeconds(delay/2);
            SoundManager.instance.PlaySoundEffect("EndLevel");
            EndOfLevel();

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

    public void SetCameraActivity(bool activity)
    {
        cameraActive = activity;
    }

    //Resets HUD 
    void ResetHUD()
    {
        ResetCombo();
        ResetScore();
        SetHighScore();
    }

    public void GoToSettings()
    {
        SwitchState(GameState.Settings);
    }

    public void GoToMainMenu()
    {
        SwitchState(GameState.MainMenu);
    }

    public void GoToControls()
    {
        SwitchState(GameState.Controls);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EndOfLevel()
    {
        SwitchState(GameState.EndOfLevel);
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
