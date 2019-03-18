using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreedingPopup : MonoBehaviour {
	public GameObject GeneticPrefab;
	public GameObject GeneticDock;

	int ICONS_IN_ROW = 3;
	float ICONS_WIDTH = 50f;
	float ICONS_HEIGHT = 50f;
	float ICONS_MARGIN = 20f;

	Dictionary<Unit, GeneticIcon> UnitToIconMap;
	List<Unit> displayedIcons;

	void Awake() {
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

	void RearrangeIcons() {
		float OFFSET_X = -(ICONS_IN_ROW * ICONS_WIDTH) / 2;

		for (int i = 0; i < displayedIcons.Count; i++) {
			GameObject portraitGO = UnitToIconMap [displayedIcons [i]].gameObject;
			int column = i % ICONS_IN_ROW;
			int row = i / ICONS_IN_ROW;
			portraitGO.transform.localPosition = new Vector3 (OFFSET_X + (column * (ICONS_WIDTH + ICONS_MARGIN)), row * -(ICONS_HEIGHT + ICONS_MARGIN), 0f);
		}
	}
}
