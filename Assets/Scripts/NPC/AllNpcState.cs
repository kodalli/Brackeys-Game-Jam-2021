using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllNpcState : MonoBehaviour {
    void Start() {
        var defeatedEnemies = GlobalControlSave.Instance.savedPlayerData.defeatedEnemyNames;
        var enemiesInScene = GetComponentsInChildren<BattleNPC>();
        foreach (var enemy in enemiesInScene) {
            foreach (var defeatedNPCName in defeatedEnemies) {
                if (enemy.GetComponent<BattleNPC>().Name == defeatedNPCName) {
                    // set enemy state to defeated
                    enemy.GetComponent<BattleNPC>().State = NPCStatus.Defeated;
                }
            }
        }
    }
}
