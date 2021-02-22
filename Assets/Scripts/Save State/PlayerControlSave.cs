using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlSave : MonoBehaviour {
    public static PlayerControlSave Instance;
    public PlayerStatistics localPlayerData = new PlayerStatistics();
    public bool ShowFPS = false;

    private int avgFrameRate;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        if (Instance != this) {
            Destroy(gameObject);
        }
        //GlobalControlSave.Instance.Player = gameObject;
        //SaveData();
    }

    private void Start() {
        localPlayerData = GlobalControlSave.Instance.savedPlayerData;
        // localPlayerData.FillBaseValues(); // local player not loading global data, quick fix
        // Debug.Log(localPlayerData.monstersDict.Count);
    }
    private void Update() {
        if (ShowFPS) FPSCounter();
    }

    public void SaveData() {
        if (GlobalControlSave.Instance != null)
            GlobalControlSave.Instance.savedPlayerData = localPlayerData;
    }

    public void DisplayMonstersDict() {
        foreach (KeyValuePair<string, Monster> monster in localPlayerData.monstersDict) {
            Debug.Log(monster.Key + ", " + monster.Value.CurHP);
        }
    }

    private void FPSCounter() {
        float current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        Debug.Log(avgFrameRate.ToString() + " FPS");
        //display_Text.text = avgFrameRate.ToString() + " FPS";
    }
}
