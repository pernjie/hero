using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreedingPopup : MonoBehaviour {
	LeftPanel leftPanel;
	BreedingManager breedingManager;
	MouseController mouseController;

	public GameObject GeneticPrefab;
	public GameObject GeneticDock;

	public BreedingSlot MaleSlot;
	public BreedingSlot FemaleSlot;

	int ICONS_IN_ROW = 3;
	float ICONS_WIDTH = 50f;
	float ICONS_HEIGHT = 50f;
	float ICONS_MARGIN = 20f;

	Dictionary<Unit, GeneticIcon> UnitToIconMap;
	List<Unit> displayedIcons;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
		breedingManager = FindObjectOfType<BreedingManager> ();
		mouseController = FindObjectOfType<MouseController> ();
		UnitToIconMap = new Dictionary<Unit, GeneticIcon> ();
		displayedIcons = new List<Unit> ();
	}

	public void AddGeneticIcon(Unit unit) {
		GameObject GeneticIconGO = Instantiate (GeneticPrefab, GeneticDock.transform);
		GeneticIconGO.GetComponent<GeneticIcon> ().Initialise (unit);
		UnitToIconMap [unit] = GeneticIconGO.GetComponent<GeneticIcon> ();
		displayedIcons.Add (unit);

		RearrangeIcons ();
	}

	public void RemoveGeneticIcon(Unit unit) {
		GeneticIcon icon = UnitToIconMap[unit];
		UnitToIconMap.Remove (unit);
		Destroy (icon.gameObject);

		displayedIcons.Remove (unit);

		RearrangeIcons ();
	}

	void RearrangeIcons() {
		float OFFSET_X = -(ICONS_IN_ROW * ICONS_WIDTH) / 2;

		for (int i = 0; i < displayedIcons.Count; i++) {
			GameObject portraitGO = UnitToIconMap [displayedIcons [i]].gameObject;
			int column = i % ICONS_IN_ROW;
			int row = i / ICONS_IN_ROW;
			portraitGO.transform.localPosition = new Vector3 (OFFSET_X + (column * (ICONS_WIDTH + ICONS_MARGIN)), row * -(ICONS_HEIGHT + ICONS_MARGIN), 0f);
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
			Unit newUnit = breedingManager.Breed (MaleSlot.iconInSlot.unit, FemaleSlot.iconInSlot.unit);
			RemoveGeneticIcon (MaleSlot.iconInSlot.unit);
			RemoveGeneticIcon (FemaleSlot.iconInSlot.unit);

			MaleSlot.RemoveIconFromSlot ();
			FemaleSlot.RemoveIconFromSlot ();

			leftPanel.HideBreedingPopup ();

			mouseController.ChangeMouseMode (MouseMode.Targeting, targetingCallbackFunc:OnPlaceUnitDone);
		}
	}

	public void OnPlaceUnitDone(Tile tile) {
		Debug.Log ("callback! " + tile.ToString());
	}
}
