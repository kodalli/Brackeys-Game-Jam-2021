using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllNpcState : MonoBehaviour {
    [SerializeField] private List<GameObject> enemies;
    // Start is called before the first frame update
    void Start() {
        var npcThatWasBattled = GlobalControlSave.Instance.savedPlayerData.enemyData;

        var defeatedEnemies = GlobalControlSave.Instance.savedPlayerData.defeatedEnemyNames;

        foreach (var enemy in enemies) {
            foreach (var defeated in defeatedEnemies) {
                if (enemy.GetComponent<BattleNPC>().Name == defeated) {
                    // set enemy state to defeated
                    //enemy.SetActive(false);
                    enemy.GetComponent<BattleNPC>().State = NPCStatus.Defeated;
                }

            }
        }

    }

}
