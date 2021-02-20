using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlSave : MonoBehaviour {
    public static PlayerControlSave Instance;
    public PlayerStatistics localPlayerData = new PlayerStatistics();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        if (Instance != this) {
            Destroy(gameObject);
        }
        SaveData();
    }

    private void Start() {
        localPlayerData = GlobalControlSave.Instance.savedPlayerData;
        // localPlayerData.FillBaseValues(); // local player not loading global data, quick fix
        // Debug.Log(localPlayerData.monstersDict.Count);
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
}
