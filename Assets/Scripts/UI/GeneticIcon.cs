using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneticIcon : MonoBehaviour {
	public Text unitName;

	public Unit unit;

	public void Initialise (Unit unit) {
		unitName.text = unit.name;
	}
}
