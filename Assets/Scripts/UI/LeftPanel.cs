using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanel : MonoBehaviour {
	public GameObject HeroPortraitPrefab;
	public GameObject GeneticPrefab;
	UnitManager unitManager;

	public GameObject PortraitsDock;
	public GameObject GeneticDock;

	public GameObject CapesView;
	public GameObject BreedingView;

	int PORTRAITS_IN_ROW = 3;
	float PORTRAIT_WIDTH = 50f;
	float PORTRAIT_HEIGHT = 50f;
	float PORTRAIT_MARGIN = 20f;

	List<Unit> displayedUnits;
	Dictionary<Unit, HeroPortrait> UnitToPortraitMap;

	void Awake() {
		unitManager = FindObjectOfType<UnitManager> ();
	}

	void Start() {
		UnitToPortraitMap = new Dictionary<Unit, HeroPortrait> ();
		displayedUnits = new List<Unit> ();
	}

	public void ToggleView(PanelView view) {
		if (view == PanelView.Capes) {
			CapesView.SetActive (true);
			BreedingView.SetActive (false);
		} else if (view == PanelView.Money) {

		} else if (view == PanelView.Breeding) {
			CapesView.SetActive (false);
			BreedingView.SetActive (true);
		} else if (view == PanelView.Laws) {

		}
	}

	public void AddPortrait(Unit unit) {
		GameObject HeroPortraitGO = Instantiate (HeroPortraitPrefab, PortraitsDock.transform);
		HeroPortraitGO.GetComponent<HeroPortrait> ().Initialise (unit);
		UnitToPortraitMap [unit] = HeroPortraitGO.GetComponent<HeroPortrait> ();
		displayedUnits.Add (unit);

		RearrangePortraits ();
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
	Breeding,
	Laws
}
