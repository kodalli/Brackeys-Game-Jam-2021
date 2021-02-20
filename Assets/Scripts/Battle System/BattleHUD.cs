using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI levelText;
	public TextMeshProUGUI hpText;
	public Slider hpSlider;
	public Image Fill;
	//private Color green;

	public void SetHUD(Monster unit)
	{
		nameText.text = unit.Name;
		levelText.text = "Lvl " + unit.GetLevel();
		hpSlider.maxValue = unit.GetMaxHP();
		hpSlider.value = unit.CurHP;
		hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
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
