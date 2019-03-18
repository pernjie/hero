using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDetailsPopup : MonoBehaviour {
	LeftPanel leftPanel;
	GameManager gameManager;
	BreedingManager breedingManager;

	Unit unit;
	public Text name;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
		gameManager = FindObjectOfType<GameManager> ();
		breedingManager = FindObjectOfType<BreedingManager> ();
	}

	public void Initialise(Unit unit) {
		this.unit = unit;
		name.text = unit.name;
	}

	public void OnCollectClick() {
		int unitCost = unit.GetCollectCost ();
		if (gameManager.money >= unitCost) {
			gameManager.SpendMoney (unitCost);

			breedingManager.AddGeneticMaterial (unit);
			leftPanel.HideUnitDetailsPopup ();
		} else {
			Debug.Log ("Not enough money for " + unitCost);
		}
	}
}
