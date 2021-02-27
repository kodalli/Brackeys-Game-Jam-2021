using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private TextMeshProUGUI hpText;
	[SerializeField] private Slider hpSlider;
	[SerializeField] private Image Fill;
	[SerializeField] private TextMeshProUGUI userName;
	[SerializeField] private bool isPlayer;
	//private Color green;

	public void SetHUD(Monster unit)
	{
		nameText.text = unit.Name;
		levelText.text = "Lvl " + unit.GetLevel();
		hpSlider.maxValue = unit.GetMaxHP();
		hpSlider.value = unit.CurHP;
		hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
		if (isPlayer) {
			userName.text = "Player";
        } else {
			userName.text = PlayerControlSave.Instance.localPlayerData.enemyData?.Name ?? "NPC test";
        }
		//ChangeColor();
	}

	public void SetHP(int hp)
	{
		hpSlider.value = hp;
		hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
		//ChangeColor();
	}

    private void LateUpdate() {
		if (hpSlider.value < hpSlider.maxValue * 0.3f) {
			Fill.color = Color.red;
		} else if (hpSlider.value < hpSlider.maxValue * 0.6f) {
			Fill.color = Color.yellow;
        } else { 
			Fill.color = Color.green; 
		}
	}

}
