using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    bool isGameOver;
    bool playerReady;
    bool initReadyScreen;

    int playerScore;

    float gameRestartTime; // How long before the scene restarts
    float gamePlayerReadyTime; // How long before the game starts while the player is frozen at the beginning

    // How long the above variables will be delayed by
    public float gameRestartDelay = 5f;
    public float gamePlayerReadyDelay = 3f;

    TextMeshProUGUI playerScoreText;
    TextMeshProUGUI screenMessageText;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    ///  Update function which checks if player is dead or not.
    ///  If player is dead, then reload the current scene
    /// </summary>
    private void Update() {

        // TODO: Make this into States in a game state machine

        if (playerReady) {
            if (initReadyScreen) {

                PlayerX.Instance.InputHandler.PlayerInput.SwitchCurrentActionMap("Empty");
                FreezePlayer(true);
                FreezeEnemies(true);

                screenMessageText.alignment = TextAlignmentOptions.Center;
                screenMessageText.alignment = TextAlignmentOptions.Top;
                screenMessageText.fontStyle = FontStyles.UpperCase;
                screenMessageText.fontSize = 64;
                screenMessageText.text = "\nREADY";
                initReadyScreen = false;
            }

            gamePlayerReadyTime -= Time.deltaTime;
            if (gamePlayerReadyTime < 0) {

                PlayerX.Instance.InputHandler.PlayerInput.SwitchCurrentActionMap("Gameplay");
                FreezePlayer(false);
                FreezeEnemies(false);

                screenMessageText.text = "";
                playerReady = false;
            }
            return;
        }

        //if (playerScoreText != null) {
        playerScoreText.text = String.Format("<mspace=\"{0}\">{1:0000000}</mspace>",
        playerScoreText.fontSize, playerScore); // mspace means monospace and equally spacing letters
        //}

        if (!isGameOver) {
            // Do stuff while the game is running
        } else {
            gameRestartTime -= Time.deltaTime;
            if (gameRestartTime < 0) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    /// <summary>
    ///  Load scene into scene manager queue
    /// </summary>
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    /// <summary>
    ///  Remove scene from scene manager queue
    /// </summary>
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    /// <summary>
    /// Initialize what the scene should do onEnable
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => StartGame();

    /// <summary>
    ///  Initializes the game by setting the appropriate flags to true and false
    ///  Initalizes the opening text and score components
    ///  Begins the opening game music
    /// </summary>
    private void StartGame() {
        isGameOver = false; // Game just started
        playerReady = true;
        initReadyScreen = true;
        gamePlayerReadyTime = gamePlayerReadyDelay;
        playerScoreText = GameObject.Find("PlayerScore").GetComponent<TextMeshProUGUI>();
        screenMessageText = GameObject.Find("ScreenMessage").GetComponent<TextMeshProUGUI>();
        SoundManager.Instance.MusicSource.Play();
    }

    /// <summary>
    /// Public function to add score to player total score
    /// </summary>
    /// <param name="points"></param>
    public void AddScorePoints(int points) => playerScore += points;

    /// <summary>
    ///  Freeze the players movement 
    /// </summary>
    /// <param name="freeze"></param>
    private void FreezePlayer(bool freeze) => PlayerX.Instance.FreezePlayer(freeze);

    /// <summary>
    /// Freeze all enemies currently present on the scene
    /// </summary>
    /// <param name="freeze"></param>
    private void FreezeEnemies(bool freeze) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            enemy.GetComponent<EnemyController>().FreezeEnemy(freeze);
        }
    }

    /// <summary>
    /// Freeze all bullets currently present on the scene
    /// </summary>
    /// <param name="freeze"></param>
    private void FreezeBullets(bool freeze) {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets) {
            bullet.GetComponent<BulletScript>().FreezeBullet(freeze);
        }
    }

    /// <summary>
    /// Sets in motion what happens once the player dies
    /// Resets all flags
    /// Stops all music
    /// Disable player movement and destroy all bullets and enemies currently on the scene
    /// </summary>
    public void PlayerDefeated() {
        isGameOver = true;
        gameRestartTime = gamePlayerReadyDelay;

        // Stop all music
        SoundManager.Instance.Stop();
        SoundManager.Instance.StopMusic();

        // Disable and Destroy everything since game is over
        PlayerX.Instance.InputHandler.PlayerInput.SwitchCurrentActionMap("Empty");
        FreezePlayer(true);
        FreezeEnemies(false);

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets) {
            Destroy(bullet);
        }

        GameObject[] explosions = GameObject.FindGameObjectsWithTag("Explosion");
        foreach (GameObject explosion in explosions) {
            Destroy(explosion);
        }
    }


}
