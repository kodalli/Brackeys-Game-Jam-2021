using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCBattleManager : MonoBehaviour
{
    private bool isNear = false;

    //public GameObject dialogBox;
    //public TextMeshProUGUI dialogText;
    //public string dialog;

    [SerializeField] private GameObject keyPrefab;
    private GameObject keyObj;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && isNear) {
            //if (dialogBox.activeInHierarchy) {
            //    dialogBox.SetActive(false);
            //}
            //else {
            //    dialogBox.SetActive(true);
            //    dialogText.text = dialog;
            //}

            // give data to player about trainer info for battle
            //var playerRef = GameObject.FindGameObjectWithTag("Player");
            //playerRef.GetComponent<PlayerController>().PlayerSceneLoad("Battle Scene");

            PlayerController.Instance.PlayerSceneLoad("Battle Scene");
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
            //dialogBox.SetActive(false);
            Destroy(keyObj);
        }
    }
}
