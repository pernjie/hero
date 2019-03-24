using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreedingPopup : MonoBehaviour {
	LeftPanel leftPanel;
	UnitManager unitManager;
	BreedingManager breedingManager;
	MouseController mouseController;

	public GameObject geneticPrefab;
	public GameObject geneticDock;

	public BreedingSlot maleSlot;
	public BreedingSlot femaleSlot;

	int ICONS_IN_ROW = 3;
	float ICONS_WIDTH = 50f;
	float ICONS_HEIGHT = 50f;
	float ICONS_MARGIN = 20f;

	Dictionary<int, GeneticIcon> sampleToIconMap;
	List<GeneticSample> displayedSamples;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
		unitManager = FindObjectOfType<UnitManager> ();
		breedingManager = FindObjectOfType<BreedingManager> ();
		mouseController = FindObjectOfType<MouseController> ();
		sampleToIconMap = new Dictionary<int, GeneticIcon> ();
		displayedSamples = new List<GeneticSample> ();
	}

	public void AddGeneticIcon(GeneticSample sample) {
		GameObject GeneticIconGO = Instantiate (geneticPrefab, geneticDock.transform);
		GeneticIconGO.GetComponent<GeneticIcon> ().Initialise (sample);
		sampleToIconMap [sample.id] = GeneticIconGO.GetComponent<GeneticIcon> ();
		displayedSamples.Add (sample);

		RearrangeIcons ();
	}

	public void RemoveGeneticIcon(int id) {
		GeneticIcon icon = sampleToIconMap[id];
		GeneticSample sample = icon.sample;

		sampleToIconMap.Remove (id);
		Destroy (icon.gameObject);
		displayedSamples.Remove (sample);

		RearrangeIcons ();
	}

	void RearrangeIcons() {
		float OFFSET_X = -(ICONS_IN_ROW * ICONS_WIDTH) / 2;

		for (int i = 0; i < displayedSamples.Count; i++) {
			GameObject iconGO = sampleToIconMap [displayedSamples [i].id].gameObject;
			int column = i % ICONS_IN_ROW;
			int row = i / ICONS_IN_ROW;
			iconGO.transform.localPosition = new Vector3 (OFFSET_X + (column * (ICONS_WIDTH + ICONS_MARGIN)), row * -(ICONS_HEIGHT + ICONS_MARGIN), 0f);
		}
	}

	public void AddIconToSlot(GeneticIcon icon, BreedingSlot slot) {
		// remove existing one first
		if (slot.iconInSlot != null) {
			slot.iconInSlot.ResetPosition ();
		}

		slot.SetIconInSlot (icon);
	}

	public void RemoveIconFromSlot(GeneticIcon icon) {
		if (maleSlot.iconInSlot == icon)
			maleSlot.RemoveIconFromSlot ();
		else if (femaleSlot.iconInSlot == icon)
			femaleSlot.RemoveIconFromSlot ();
	}

	public void OnCombineClick() {
		if (maleSlot.iconInSlot != null && femaleSlot.iconInSlot != null) {
			geneticToAdd = breedingManager.Breed (maleSlot.iconInSlot.sample.unit, femaleSlot.iconInSlot.sample.unit);
			RemoveGeneticIcon (maleSlot.iconInSlot.sample.id);
			RemoveGeneticIcon (femaleSlot.iconInSlot.sample.id);

			maleSlot.RemoveIconFromSlot ();
			femaleSlot.RemoveIconFromSlot ();

			leftPanel.HideBreedingPopup ();

			mouseController.ChangeMouseMode (MouseMode.Targeting, targetingCallbackFunc:OnPlaceUnitDone);
		}
	}

	void resetUI() {
		foreach (Transform child in geneticDock.transform) {
			GameObject.Destroy (child.gameObject);
		}
		sampleToIconMap = new Dictionary<int, GeneticIcon> ();

		foreach (GeneticSample sample in displayedSamples) {
			AddGeneticIcon (sample);
		}
	}

	GeneticMaterial geneticToAdd;
	public void OnPlaceUnitDone(Tile tile) {
		Hero newHero = new Hero ("Bred Hero");
		newHero.Initialise (geneticToAdd);
		unitManager.AddHero (newHero, tile);
	}
}
