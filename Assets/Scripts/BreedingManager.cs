using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedingManager : MonoBehaviour {
	UnitManager unitManager;
	List<Unit> GeneticOwners;

	public BreedingPopup breedingPopup;

	void Awake() {
		unitManager = FindObjectOfType<UnitManager> ();
	}

	void Start() {
		GeneticOwners = new List<Unit> ();
	}

	public void AddGeneticMaterial(Unit owner) {
		GeneticOwners.Add (owner);
		breedingPopup.AddGeneticIcon (owner);
	}

	public void SpendGeneticMaterial(Unit owner) {
		GeneticOwners.Remove (owner);
	}

	public Unit Breed(Unit unit1, Unit unit2) {
		return new Unit ();
	}
}

public class GeneticMaterial {
	public List<Codon> codons;
}

public class Codon {
	public string code;
}
