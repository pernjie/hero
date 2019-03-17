using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroPortrait : MonoBehaviour {
	Unit unit;
	public Text name;
	public Text health;

	public void Initialise(Unit unit) {
		this.unit = unit;
		name.text = unit.name;
		UpdateHealth ();
	}

	public void UpdateHealth() {
		health.text = unit.currentHealth + "/" + unit.maxHealth;

		if (unit.currentHealth == 0) {
			GetComponentInChildren<Image>().color = new Color(.2f, .2f, .2f);
		}
	}
}
