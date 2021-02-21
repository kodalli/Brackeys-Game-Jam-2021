using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MenuType { ItemMenu, SquadMenu }
public class PopulateGrid : MonoBehaviour {
    [SerializeField] private GameObject prefab; // This is our prefab object that will be exposed in the inspector
    [SerializeField] private GameObject battleSystemRef;
    [SerializeField] private GameObject menuRef;
    [SerializeField] private MenuType Menu; 

    void Start() {
        switch(Menu) {
            case MenuType.ItemMenu:
                PopulateItems();
                break;
            case MenuType.SquadMenu:
                PopulateSquad();
                break;
            default:
                break;
        }
    }

    //private void Update()
    //{
    //    // doesn't work without this update method
    //}

    void PopulateItems() {
        GameObject newObj; // Create GameObject instance

        var items = PlayerControlSave.Instance.localPlayerData.playerItems;
        // Debug.Log(items.Count);

        for (int i = 0; i < items.Count; i++) {
            // Create new instances of our prefab until we've created as many as we specified
            newObj = Instantiate(prefab, transform);
            var texts = newObj.GetComponentsInChildren<TextMeshProUGUI>();

            var imgRef = newObj.GetComponentInChildren<Image>();
            if (items[i].sprite != null) imgRef.sprite = items[i].sprite;

            var buttonRef = newObj.GetComponentInChildren<Button>();
            var item = items[i];
            buttonRef.onClick.AddListener(delegate { OnClick(item); });
            buttonRef.onClick.AddListener(delegate { menuRef.SetActive(false); });

            // 0 button name, 1 item name, 2 description
            for (var j = 0; j < texts.Length; j++) {
                switch (j) {
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

    void PopulateSquad() {
        GameObject newObj; // Create GameObject instance

        var monsterDict = PlayerControlSave.Instance.localPlayerData.monstersDict;

        //var count = 0;
        foreach(KeyValuePair<string, Monster> monster in monsterDict) {
            // Create new instances of our prefab until we've created as many as we specified
            newObj = Instantiate(prefab, transform);

            var m = monster.Value;

            var imgRef = newObj.GetComponentInChildren<Image>();
            imgRef = m.GetPrefab().GetComponent<Image>(); 

            var texts = newObj.GetComponentsInChildren<TextMeshProUGUI>();

            var buttonRef = newObj.GetComponentInChildren<Button>();
            buttonRef.onClick.AddListener(delegate { OnClick(m); });
            buttonRef.onClick.AddListener(delegate { menuRef.SetActive(false); });

            // 0 button name, 1 monster name, 2 description
            for (var j = 0; j < texts.Length; j++) {
                switch (j) {
                    case 1:
                        texts[j].text = m.Name;
                        break;
                    case 2:
                        texts[j].text = $"{m.CurHP}/{m.GetMaxHP()}HP, LvL {m.GetLevel()}, {m.GetCurXP()}/{m.GetXP(m.GetLevel()+1)}XP to next level";
                        break;
                    default:
                        break;
                }
            }
            //count++;
        }
    }

    public void OnClick(ItemData item) {
        var obj = battleSystemRef.GetComponent<BattleSystem>();
        obj.OnFuseButton(item);
    }

    public void OnClick(Monster monster) {
        var obj = battleSystemRef.GetComponent<BattleSystem>();
        obj.OnSquadSelectButton(monster.Name);
    }
}