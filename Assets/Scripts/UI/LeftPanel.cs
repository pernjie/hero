using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanel : MonoBehaviour {
	public GameObject UnitPortraitPrefab;
	UnitManager unitManager;

	public GameObject PortraitsDock;
	public GameObject UnitDetailsPopup;
	public GameObject BreedingPopup;

	public GameObject CapesView;

	int PORTRAITS_IN_ROW = 3;
	float PORTRAIT_WIDTH = 50f;
	float PORTRAIT_HEIGHT = 50f;
	float PORTRAIT_MARGIN = 20f;

	List<Unit> displayedUnits;
	Dictionary<Unit, UnitPortrait> UnitToPortraitMap;

	void Awake() {
		unitManager = FindObjectOfType<UnitManager> ();
		UnitToPortraitMap = new Dictionary<Unit, UnitPortrait> ();
		displayedUnits = new List<Unit> ();
	}

	public void ToggleView(PanelView view) {
		if (view == PanelView.Capes) {
			CapesView.SetActive (true);
		} else if (view == PanelView.Money) {
			
		} else if (view == PanelView.Laws) {

		}
	}

	public GameObject blockingPanel;
	GameObject currentPopup;

	public void ShowUnitDetailsPopup(Unit unit) {
		UnitDetailsPopup.GetComponent<UnitDetailsPopup> ().Initialise (unit);
		blockingPanel.SetActive (true);
		UnitDetailsPopup.transform.localScale = new Vector3(1f, 1f, 1f);
		currentPopup = UnitDetailsPopup;
	}

	public void HideUnitDetailsPopup() {
		blockingPanel.SetActive (false);
		UnitDetailsPopup.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	public void ShowBreedingPopup() {
		blockingPanel.SetActive (true);
		BreedingPopup.transform.localScale = new Vector3(1f, 1f, 1f);
		currentPopup = BreedingPopup;
	}

	public void HideBreedingPopup() {
		blockingPanel.SetActive (false);
		BreedingPopup.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	public void AddPortrait(Unit unit) {
		GameObject UnitPortraitGO = Instantiate (UnitPortraitPrefab, PortraitsDock.transform);
		UnitPortraitGO.GetComponent<UnitPortrait> ().Initialise (unit);
		UnitToPortraitMap [unit] = UnitPortraitGO.GetComponent<UnitPortrait> ();
		displayedUnits.Add (unit);

		RearrangePortraits ();
	}

	public void ExitCurrentPopup() {
		if (currentPopup != null) {
			blockingPanel.SetActive (false);
			currentPopup.transform.localScale = new Vector3 (0f, 0f, 0f);
		}
	}

	public void UpdateHealth(Unit unit) {
		UnitToPortraitMap [unit].UpdateHealth ();
	}

	void RearrangePortraits() {
		float OFFSET_X = -(PORTRAITS_IN_ROW * PORTRAIT_WIDTH) / 2;

		for (int i = 0; i < displayedUnits.Count; i++) {
			GameObject portraitGO = UnitToPortraitMap [displayedUnits [i]].gameObject;
			int column = i % PORTRAITS_IN_ROW;
			int row = i / PORTRAITS_IN_ROW;
			portraitGO.transform.localPosition = new Vector3 (OFFSET_X + (column * (PORTRAIT_WIDTH + PORTRAIT_MARGIN)), row * -(PORTRAIT_HEIGHT + PORTRAIT_MARGIN), 0f);
		}
	}
}

public enum PanelView {
	Capes,
	Money,
	Laws
}
