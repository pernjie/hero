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
	public Text health;
	public Text strength;
	public Text speed;
	public Text technique;
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
		name.text = unit.name;

		if (unit.gender == Gender.Male) {
			name.color = new Color (.2f, .2f, 1f);
		} else {
			name.color = new Color (1f, .2f, .2f);
		}

		//gender.text = unit.gender.ToString();
		health.text = "Health: " + unit.genetics.health;
		strength.text = "Strength: " + unit.genetics.strength;
		speed.text = "Speed: " + unit.genetics.speed;
		technique.text = "Technique: " + unit.genetics.technique;
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
