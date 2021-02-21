using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private ItemData item;
    [SerializeField] private GameObject keyPrefab;
    private GameObject keyObj;
    private bool isNear;
    private List<string> dialog = new List<string>();
    private void Start() {
        dialog.Add($"You picked up {item.itemName}!");
        dialog.Add("Fuse it with your homies to make them stronger");
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
        // duplicate items still added fix later
        PlayerControlSave.Instance.localPlayerData.playerItems.Add(item);
        gameObject.SetActive(false);
    }
}
