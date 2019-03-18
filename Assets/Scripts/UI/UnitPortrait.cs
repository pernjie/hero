using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour {
	LeftPanel leftPanel;
	UnitManager unitManager;

	Unit unit;
	public Text name;
	public Text health;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
		unitManager = FindObjectOfType<UnitManager> ();
	}

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

	public void OnClick() {
		leftPanel.ShowUnitDetailsPopup (unit);
	}
}
