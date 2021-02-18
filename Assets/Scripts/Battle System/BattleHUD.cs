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


	public void SetHUD(Monster unit)
	{
		nameText.text = unit.Name;
		levelText.text = "Lvl " + unit.GetLevel();
		hpSlider.maxValue = unit.GetMaxHP();
		hpSlider.value = unit.CurHP;
		hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
	}

	public void SetHP(int hp)
	{
		hpSlider.value = hp;
		hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
	}

}
