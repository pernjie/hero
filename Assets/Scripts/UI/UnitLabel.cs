using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitLabel : MonoBehaviour {
	public Text nameText;

	Vector3 offset = new Vector3(0f, 20f, 0f);

	Unit unit;
	GameObject unitToTrack;

	public void Initialise(Unit unit, GameObject unitToTrack) {
		nameText.text = unit.name;
		this.unitToTrack = unitToTrack;
	}

	void Update() {
		if (unitToTrack != null) {
			transform.position = Camera.main.WorldToScreenPoint (unitToTrack.transform.position) + offset;
		}
	}
}
