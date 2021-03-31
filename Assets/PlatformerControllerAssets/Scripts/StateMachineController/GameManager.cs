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

        if (playerScoreText != null) {
            playerScoreText.text = String.Format("<mspace=\"{0}\">{1:0000000}</mspace>",
            playerScoreText.fontSize, playerScore); // mspace means monospace and equally spacing letters
        }

        if (!isGameOver) {
            // Do stuff while the game is running
        } else {
            gameRestartTime -= Time.deltaTime;
            if (gameRestartTime < 0) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => StartGame();
    private void StartGame() {
        isGameOver = false; // Game just started
        playerReady = true;
        initReadyScreen = true;
        gamePlayerReadyTime = gamePlayerReadyDelay;
        playerScoreText = GameObject.Find("PlayerScore").GetComponent<TextMeshProUGUI>();
        screenMessageText = GameObject.Find("ScreenMessage").GetComponent<TextMeshProUGUI>();
        SoundManager.Instance.MusicSource.Play();
    }

    public void AddScorePoints(int points) => playerScore += points;

    private void FreezePlayer(bool freeze) => PlayerX.Instance.FreezePlayer(freeze);
    private void FreezeEnemies(bool freeze) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            enemy.GetComponent<EnemyController>().FreezeEnemy(freeze);
        }
    }
    private void FreezeBullets(bool freeze) {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets) {
            bullet.GetComponent<BulletScript>().FreezeBullet(freeze);
        }
    }
    public void PlayerDefeated() {
        isGameOver = true;
        gameRestartTime = gamePlayerReadyDelay;

        SoundManager.Instance.Stop();
        SoundManager.Instance.StopMusic();

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
