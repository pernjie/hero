using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour {
	LeftPanel leftPanel;

	Unit unit;
	public Text nameText;
	public Text healthText;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
	}

	public void Initialise(Unit unit) {
		this.unit = unit;
		nameText.text = unit.name;
		UpdateHealth ();
	}

	public void UpdateHealth() {
		healthText.text = unit.currentHealth + "/" + unit.maxHealth;

		if (unit.currentHealth == 0) {
			GetComponentInChildren<Image>().color = new Color(.2f, .2f, .2f);
		}
	}

	public void OnClick() {
		leftPanel.ShowUnitDetailsPopup (unit, true);
	}
}
