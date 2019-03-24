using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDetailsPopup : MonoBehaviour {
	LeftPanel leftPanel;
	GameManager gameManager;
	BreedingManager breedingManager;

	Unit unit;
	public Text nameText;
	public Text healthText;
	public Text strengthText;
	public Text speedText;
	public Text techniqueText;
	public Button button;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
		gameManager = FindObjectOfType<GameManager> ();
		breedingManager = FindObjectOfType<BreedingManager> ();
	}

	public void ToggleButton(bool toShow) {
		button.gameObject.SetActive (toShow);
	}

	public void Initialise(Unit unit) {
		this.unit = unit;
		nameText.text = unit.name;

		if (unit.gender == Gender.Male) {
			nameText.color = new Color (.2f, .2f, 1f);
		} else {
			nameText.color = new Color (1f, .2f, .2f);
		}

		//gender.text = unit.gender.ToString();
		healthText.text = "Health: " + unit.genetics.health;
		strengthText.text = "Strength: " + unit.genetics.strength;
		speedText.text = "Speed: " + unit.genetics.speed;
		techniqueText.text = "Technique: " + unit.genetics.technique;
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
