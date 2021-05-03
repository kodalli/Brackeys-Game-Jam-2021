// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class Sign : MonoBehaviour {
//     [SerializeField] private string dialog;
//     [SerializeField] private GameObject keyPrefab;
//     private GameObject keyObj;

//     public void EnableKey() {
//         Vector3 pos = transform.position;
//         pos.y += 1f; // hover above
//         keyObj = Instantiate(keyPrefab, pos, Quaternion.identity);
//     }

//     public void DisableKey() {
//         Dialog.Instance.dialogBox.SetActive(false);
//         Destroy(keyObj);
//     }

//     public void OnInteractKey() {
//         if (Dialog.Instance.signBox.activeSelf) {
//             Dialog.Instance.signBox.SetActive(false);
//         }
//         else {
//             //signBox.SetActive(true);
//             //dialogText.text = dialog;
//             Dialog.Instance.DisplaySignText(dialog);
//         }
//     }
// }
