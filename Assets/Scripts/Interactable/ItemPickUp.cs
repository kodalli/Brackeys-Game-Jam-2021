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
    [SerializeField] private PopulateGrid itemMenu;
    [SerializeField] private PopulateGrid squadMenu;

    private GameObject keyObj;
    //private bool isNear;
    private List<string> dialog = new List<string>();
    private void Start() {
        if (PickupType == ItemType.FusionItem && item != null) {
            dialog.Add($"You picked up {item.itemName}!");
            dialog.Add("Fuse it with your homies to make them stronger");
        } else if (PickupType == ItemType.Monster && monster != null) {
            dialog.Add($"You picked up {monster.Name}!");
        }

    }

    public void EnableKey() {
        Vector3 pos = transform.position;
        pos.y += 1f; // hover above
        keyObj = Instantiate(keyPrefab, pos, Quaternion.identity);
    }

    public void DisableKey() {
        Dialog.Instance.dialogBox.SetActive(false);
        Destroy(keyObj);
    }

    public void AddToInventory() {
        if (Dialog.Instance.dialogBox.activeSelf) {
            return;
        }

        Dialog.Instance.DisplayTextInDialogueBox(dialog);
        Debug.Log("picked");

        // check duplicates then add
        if (PickupType == ItemType.FusionItem && item != null) {
            if (PlayerControlSave.Instance.localPlayerData.playerItems
                .Any(other => other.itemName == item.itemName)) return;
            PlayerControlSave.Instance.localPlayerData.playerItems.Add(item);
            itemMenu.UpdateMenu();
        }
        else if (PickupType == ItemType.Monster && monster != null) {
            if (PlayerControlSave.Instance.localPlayerData.monstersDict
                .ContainsKey(monster.Name)) return;
            PlayerControlSave.Instance.localPlayerData.squad.Add(monster);
            var mon = new Monster(monster);
            mon.LevelUp(monsterLevel);
            PlayerControlSave.Instance.localPlayerData.monstersDict.Add(mon.Name, mon);
            squadMenu.UpdateMenu();
        }

        gameObject.SetActive(false);
    }
}
