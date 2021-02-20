using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sign : MonoBehaviour
{
    private bool isNear = false;

    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    public string dialog;

    [SerializeField] private GameObject key;
    private GameObject temp;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isNear)
        {
            if (dialogBox.activeInHierarchy)
            {
                dialogBox.SetActive(false);
            }
            else
            {
                dialogBox.SetActive(true);
                dialogText.text = dialog;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isNear = true;
            Vector3 pos = transform.position;
            pos.y += 1f; // hover above
            temp = Instantiate(key, pos, Quaternion.identity);

        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isNear = false;
            dialogBox.SetActive(false);
            Destroy(temp);
        }
    }

}
