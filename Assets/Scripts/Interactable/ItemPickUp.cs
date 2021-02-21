using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ItemType { FusionItem, Monster}
public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private ItemData item;
    [SerializeField] private MonsterScriptableObject monster;
    [SerializeField] private int monsterLevel;
    [SerializeField] private ItemType PickupType;
    [SerializeField] private GameObject keyPrefab;
    private GameObject keyObj;
    private bool isNear;
    private List<string> dialog = new List<string>();
    private void Start() {
        if (PickupType == ItemType.FusionItem && item != null) {
            dialog.Add($"You picked up {item.itemName}!");
            dialog.Add("Fuse it with your homies to make them stronger");
        } else if (PickupType == ItemType.Monster && monster != null) {
            dialog.Add($"You picked up {monster.Name}!");
        }

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && isNear) {
            Dialog.Instance.DisplayTextInDialogueBox(dialog);
            AddToInventory();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            isNear = true;
            Vector3 pos = transform.position;
            pos.y += 1f; // hover above
            keyObj = Instantiate(keyPrefab, pos, Quaternion.identity);

        }

    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            isNear = false;
            Destroy(keyObj);
        }
    }

    private void AddToInventory() {
        // check duplicates then add
        if (PickupType == ItemType.FusionItem && item != null) {
            if (PlayerControlSave.Instance.localPlayerData.playerItems.Any(other => other.itemName == item.itemName)) {
                PlayerControlSave.Instance.localPlayerData.playerItems.Add(item);
            }
        }
        else if (PickupType == ItemType.Monster && monster != null) {
            if (PlayerControlSave.Instance.localPlayerData.monstersDict.ContainsKey(monster.Name)) return;
            PlayerControlSave.Instance.localPlayerData.squad.Add(monster);
            var mon = new Monster(monster);
            mon.LevelUp(monsterLevel);
            PlayerControlSave.Instance.localPlayerData.monstersDict.Add(mon.Name, mon);
        }
        gameObject.SetActive(false);
    }
}
