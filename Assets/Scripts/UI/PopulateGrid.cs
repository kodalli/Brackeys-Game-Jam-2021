using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopulateGrid : MonoBehaviour
{
	[SerializeField] private GameObject prefab; // This is our prefab object that will be exposed in the inspector
    [SerializeField] private GameObject battleSystemRef;
    [SerializeField] private GameObject itemMenuRef;

	void Start()
	{
        Populate();
	}

    private void Update()
    {
        
    }

    void Populate()
	{
		GameObject newObj; // Create GameObject instance

		var items = PlayerControlSave.Instance.localPlayerData.playerItems;
        // Debug.Log(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            // Create new instances of our prefab until we've created as many as we specified
            newObj = (GameObject)Instantiate(prefab, transform);
            var texts = newObj.GetComponentsInChildren<TextMeshProUGUI>();

            
            var buttonRef = newObj.GetComponentInChildren<Button>();
            var item = items[i];
            buttonRef.onClick.AddListener(delegate { OnClick(item); });
            buttonRef.onClick.AddListener(delegate { itemMenuRef.SetActive(false); });

            // 0 button name, 1 item name, 2 description
            for (var j = 0; j < texts.Length; j++)
            {
                switch(j)
                {
                    case 1:
                        texts[j].text = item.itemName;
                        break;
                    case 2:
                        texts[j].text = item.itemDescription;
                        break;
                    default:
                        break;
                }
            }
            //Debug.Log(texts);

        }
    }

    public void OnClick(ItemData item)
    {
        var obj = battleSystemRef.GetComponent<BattleSystem>();
        obj.OnFuseButton(item);
    }
}