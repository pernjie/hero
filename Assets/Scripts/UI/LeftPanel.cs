using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanel : MonoBehaviour {
	public GameObject unitPortraitPrefab;
	UnitManager unitManager;

	public GameObject portraitsDock;
	public GameObject unitDetailsPopup;
	public GameObject breedingPopup;
	public Text money;

	public GameObject capesView;

	int PORTRAITS_IN_ROW = 3;
	float PORTRAIT_WIDTH = 50f;
	float PORTRAIT_HEIGHT = 50f;
	float PORTRAIT_MARGIN = 20f;

	List<Unit> displayedUnits;
	Dictionary<Unit, UnitPortrait> unitToPortraitMap;

	public GameObject unitLabelPrefab;
	List<GameObject> unitLabels;

	void Awake() {
		unitManager = FindObjectOfType<UnitManager> ();
		unitToPortraitMap = new Dictionary<Unit, UnitPortrait> ();
		displayedUnits = new List<Unit> ();
	}

	void Start() {
		unitLabels = new List<GameObject> ();
		HideUnitDetailsPopup ();
		HideBreedingPopup ();
	}

	public void ToggleView(PanelView view) {
		if (view == PanelView.Capes) {
			capesView.SetActive (true);
		} else if (view == PanelView.Money) {
			
		} else if (view == PanelView.Laws) {

		}
	}

	public GameObject blockingPanel;
	public GameObject currentPopup;

	public void ShowUnitDetailsPopup(Unit unit, bool showButton) {
		unitDetailsPopup.GetComponent<UnitDetailsPopup> ().ToggleButton (showButton);
		unitDetailsPopup.GetComponent<UnitDetailsPopup> ().Initialise (unit);
		//blockingPanel.SetActive (true);
		unitDetailsPopup.transform.localScale = new Vector3(1f, 1f, 1f);

		ShowUnitLabel (unit);
	}

	public void HideUnitDetailsPopup() {
		//blockingPanel.SetActive (false);
		unitDetailsPopup.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	public void ShowBreedingPopup() {
		// clear first
		HideUnitDetailsPopup ();
		HideUnitLabels ();

		blockingPanel.SetActive (true);
		breedingPopup.transform.localScale = new Vector3(1f, 1f, 1f);
		currentPopup = breedingPopup;
	}

	public void HideBreedingPopup() {
		blockingPanel.SetActive (false);
		HideUnitDetailsPopup ();
		breedingPopup.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	public void AddPortrait(Unit unit) {
		GameObject UnitPortraitGO = Instantiate (unitPortraitPrefab, portraitsDock.transform);
		UnitPortraitGO.GetComponent<UnitPortrait> ().Initialise (unit);
		unitToPortraitMap [unit] = UnitPortraitGO.GetComponent<UnitPortrait> ();
		displayedUnits.Add (unit);

		RearrangePortraits ();
	}

	public void ExitCurrentPopup() {
		if (currentPopup != null) {
			blockingPanel.SetActive (false);
			currentPopup.transform.localScale = new Vector3 (0f, 0f, 0f);
			currentPopup = null;
		}
	}

	public void UpdateHealth(Unit unit) {
		unitToPortraitMap [unit].UpdateHealth ();
	}

	void RearrangePortraits() {
		float OFFSET_X = -(PORTRAITS_IN_ROW * PORTRAIT_WIDTH) / 2;

		for (int i = 0; i < displayedUnits.Count; i++) {
			GameObject portraitGO = unitToPortraitMap [displayedUnits [i]].gameObject;
			int column = i % PORTRAITS_IN_ROW;
			int row = i / PORTRAITS_IN_ROW;
			portraitGO.transform.localPosition = new Vector3 (OFFSET_X + (column * (PORTRAIT_WIDTH + PORTRAIT_MARGIN)), row * -(PORTRAIT_HEIGHT + PORTRAIT_MARGIN), 0f);
		}
	}

	public void UpdateMoneyUI(int amount) {
		money.text = "Money: " + amount;
	}

	public void HideUnitLabels() {
		//clear previous label
		foreach (GameObject previousLabel in unitLabels) {
			Destroy (previousLabel.gameObject);
		}
		unitLabels.Clear ();
	}

	public void ShowUnitLabel(Unit unit) {
		HideUnitLabels ();

		GameObject unitGO = unitManager.GetUnitGOFromUnit (unit);
		GameObject unitLabel = Instantiate (unitLabelPrefab, transform);
		unitLabel.GetComponent<UnitLabel> ().Initialise (unit, unitGO);

		unitLabels.Add (unitLabel);
	}
}

public enum PanelView {
	Capes,
	Money,
	Laws
}
