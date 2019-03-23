using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreedingPopup : MonoBehaviour {
	LeftPanel leftPanel;
	UnitManager unitManager;
	BreedingManager breedingManager;
	MouseController mouseController;

	public GameObject GeneticPrefab;
	public GameObject GeneticSlotPrefab;
	public GameObject GeneticDock;

	public BreedingSlot MaleSlot;
	public BreedingSlot FemaleSlot;

	int ICONS_IN_ROW = 3;
	float ICONS_WIDTH = 50f;
	float ICONS_HEIGHT = 50f;
	float ICONS_MARGIN = 20f;

	Dictionary<Unit, GeneticIcon> UnitToIconMap;
	Dictionary<Unit, GeneticIcon> UnitToIconSlotMap;
	List<GeneticSample> displayedSamples;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
		unitManager = FindObjectOfType<UnitManager> ();
		breedingManager = FindObjectOfType<BreedingManager> ();
		mouseController = FindObjectOfType<MouseController> ();
		UnitToIconMap = new Dictionary<Unit, GeneticIcon> ();
		UnitToIconSlotMap = new Dictionary<Unit, GeneticIcon> ();
		displayedSamples = new List<GeneticSample> ();
	}

	public void AddGeneticIcon(GeneticSample sample) {
		GameObject GeneticIconSlotGO = Instantiate (GeneticSlotPrefab, GeneticDock.transform);
		GameObject GeneticIconGO = Instantiate (GeneticPrefab, GeneticDock.transform);
		GeneticIconSlotGO.GetComponent<GeneticIcon> ().Initialise (sample, false);
		GeneticIconGO.GetComponent<GeneticIcon> ().Initialise (sample, true);
		UnitToIconMap [sample.unit] = GeneticIconGO.GetComponent<GeneticIcon> ();
		UnitToIconSlotMap [sample.unit] = GeneticIconSlotGO.GetComponent<GeneticIcon> ();
		displayedSamples.Add (sample);

		RearrangeIcons ();
	}

	public void UpdateGeneticIcon(GeneticSample updatedSample) {
		UnitToIconSlotMap [updatedSample.unit].Initialise (updatedSample, false);
		UnitToIconMap [updatedSample.unit].Initialise (updatedSample, true);

		foreach (GeneticSample sample in displayedSamples) {
			if (sample.unit.uid == updatedSample.unit.uid) {
				sample.samples = updatedSample.samples;
				break;
			}
		}
	}

	public void RemoveGeneticIcon(Unit unit) {
		GeneticIcon icon = UnitToIconMap[unit];
		GeneticSample sample = icon.sample;
		// TODO: resolve this if there are multiple of the same entry

		if (sample.samples > 1) {
			sample.samples -= 1;
			UpdateGeneticIcon (sample);
		} else {
			UnitToIconMap.Remove (unit);
			UnitToIconSlotMap.Remove (unit);
			Destroy (icon.gameObject);
			displayedSamples.Remove (sample);
		}

		RearrangeIcons ();
	}

	void RearrangeIcons() {
		float OFFSET_X = -(ICONS_IN_ROW * ICONS_WIDTH) / 2;

		for (int i = 0; i < displayedSamples.Count; i++) {
			GameObject iconGO = UnitToIconMap [displayedSamples [i].unit].gameObject;
			GameObject iconSlotGO = UnitToIconSlotMap [displayedSamples [i].unit].gameObject;
			int column = i % ICONS_IN_ROW;
			int row = i / ICONS_IN_ROW;
			iconGO.transform.localPosition = new Vector3 (OFFSET_X + (column * (ICONS_WIDTH + ICONS_MARGIN)), row * -(ICONS_HEIGHT + ICONS_MARGIN), 0f);
			iconSlotGO.transform.localPosition = new Vector3 (OFFSET_X + (column * (ICONS_WIDTH + ICONS_MARGIN)), row * -(ICONS_HEIGHT + ICONS_MARGIN), 0f);
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
		if (MaleSlot.iconInSlot == icon)
			MaleSlot.RemoveIconFromSlot ();
		else if (FemaleSlot.iconInSlot == icon)
			FemaleSlot.RemoveIconFromSlot ();
	}

	public void OnCombineClick() {
		if (MaleSlot.iconInSlot != null && FemaleSlot.iconInSlot != null) {
			geneticToAdd = breedingManager.Breed (MaleSlot.iconInSlot.sample.unit, FemaleSlot.iconInSlot.sample.unit);
			RemoveGeneticIcon (MaleSlot.iconInSlot.sample.unit);
			RemoveGeneticIcon (FemaleSlot.iconInSlot.sample.unit);

			MaleSlot.RemoveIconFromSlot ();
			FemaleSlot.RemoveIconFromSlot ();

			leftPanel.HideBreedingPopup ();

			mouseController.ChangeMouseMode (MouseMode.Targeting, targetingCallbackFunc:OnPlaceUnitDone);
		}
	}

	void resetUI() {
		foreach (Transform child in GeneticDock.transform) {
			GameObject.Destroy (child.gameObject);
		}
		UnitToIconMap = new Dictionary<Unit, GeneticIcon> ();
		UnitToIconSlotMap = new Dictionary<Unit, GeneticIcon> ();

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
