﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BreedingManager : MonoBehaviour {
	List<GeneticSample> geneticSamples;

	public List<Power> powerList;
	public char[] codonList;
	int NUM_CODONS = 20;
	float MUTATION_RATE = 0.05f;
	float MUTATION_CHANCE_POWER = 0.04f;

	public BreedingPopup breedingPopup;

	void Awake() {
		geneticSamples = new List<GeneticSample> ();
		//powerList = new List<Power> (Resources.LoadAll ("Powers", typeof(Power)).Cast<Power> ().ToArray ());

		codonList = "pern".ToCharArray ();
	}

	void Start() {
		
	}

	public void AddGeneticMaterial(Unit owner) {
		// TODO: check if clash
		int sampleId = Random.Range (0, 9999);
		GeneticSample newSample = new GeneticSample (owner, sampleId);
		geneticSamples.Add (newSample);
		breedingPopup.AddGeneticIcon (newSample);
	}

	public void SpendGeneticMaterial(int sampleId) {
		GeneticSample toRemove = null;
		foreach (GeneticSample sample in geneticSamples) {
			if (sample.id == sampleId) {
				toRemove = sample;
			}
		}

		if (toRemove != null) {
			geneticSamples.Remove (toRemove);
		}
	}

	public Hero GetNewHero() {
		Hero newHero = new Hero ("Hero " + Random.Range (0, 9999));
		newHero.Initialise(GenerateInitialMaterial ());
		newHero.unitType = UnitType.Hero;

		return newHero;
	}

	public Villain GetNewVillain() {
		Villain newVillain = new Villain ("Villain " + Random.Range (0, 9999));
		newVillain.Initialise(GenerateInitialMaterial ());
		newVillain.unitType = UnitType.Villain;

		return newVillain;
	}

	public GeneticMaterial Breed(Unit unit1, Unit unit2) {
		List<Codon> newCodons = new List<Codon> ();
		for (int i = 0; i < NUM_CODONS; i++) {
			if (Random.Range (0, 2) == 0) {
				newCodons.Add (MutateCodon (unit1.genetics.codons [i].code));
			} else {
				newCodons.Add (MutateCodon (unit1.genetics.codons [i].code));
			}
		}

		GeneticMaterial gm = new GeneticMaterial(newCodons);
		Debug.Log ("Health: " + gm.health + "\n" + "Strength: " + gm.strength + "\n" + "Speed: " + gm.speed + "\n" + "Technique: " + gm.technique);

		return gm;
	}

	Codon MutateCodon(string code) {
		return new Codon (code);
	}

	public GeneticMaterial GenerateInitialMaterial() {
		List<Codon> codons = new List<Codon>();

		// generate codons randomly at first
		for (int i = 0; i < NUM_CODONS; i++) {
			codons.Add(GetRandomCodon());
		}

		GeneticMaterial gm = new GeneticMaterial(codons);

		return gm;
	}

	public Codon GetRandomCodon() {
		string code = "";
		for (int i = 0; i < 3; i++) {
			if (Random.Range (0f, 1f) < MUTATION_CHANCE_POWER) {
				code += "x";
			} else {
				code += codonList [Random.Range (0, 4)];
			}
		}

		return new Codon(code);
	}
}

public class GeneticSample {
	public Unit unit;
	public int id;

	public GeneticSample(Unit unit, int id) {
		this.unit = unit;
		this.id = id;
	}
}

public class GeneticMaterial {
	public string codonString;
	public List<Codon> codons;
	public List<Power> powers;

	public int health;
	public int strength;
	public int speed;
	public int technique;
	public float fleeThreshold;
	public int regenerationRate; // how many health per second
	public float fleeingDuration; // how long to run before finish fleeing

	public GeneticMaterial(List<Codon> codons) {
		// TODO: handle male/female difference?
		int BASE_HEALTH = 50;
		int VARIATION_HEALTH = 5;
		int BASE_STRENGTH = 3;
		int VARIATION_STRENGTH = 2;
		int BASE_SPEED = 9;
		int VARIATION_SPEED = 1;
		int BASE_TECHNIQUE = 2;
		int VARIATION_TECHNIQUE = 1;

		health = BASE_HEALTH;
		strength = BASE_STRENGTH;
		speed = BASE_SPEED;
		technique = BASE_TECHNIQUE;
		fleeThreshold = 0.2f;
		regenerationRate = 1;
		fleeingDuration = 5f;

		this.codons = codons;
		this.codonString = "";
		foreach (Codon codon in codons) {
			this.codonString += codon.code;
			if (!codon.isPower) {
				if (codon.code [0] == 'p') {
					// health stat
					if (codon.code [1] == 'p') {
						// nothing
					} else if (codon.code [1] == 'e') {
						health += VARIATION_HEALTH * 1;
					} else if (codon.code [1] == 'r') {
						health += VARIATION_HEALTH * 2;
					} else if (codon.code [1] == 'n') {
						health += VARIATION_HEALTH * 3;
					}
				} else if (codon.code [0] == 'e') {
					// strength stat
					if (codon.code [1] == 'p') {
						strength += VARIATION_STRENGTH * 3;
					} else if (codon.code [1] == 'e') {
						// nothing
					} else if (codon.code [1] == 'r') {
						strength += VARIATION_STRENGTH * 1;
					} else if (codon.code [1] == 'n') {
						strength += VARIATION_STRENGTH * 2;
					}
				} else if (codon.code [0] == 'r') {
					// speed stat
					if (codon.code [1] == 'p') {
						speed += VARIATION_SPEED * 2;
					} else if (codon.code [1] == 'e') {
						speed += VARIATION_SPEED * 3;
					} else if (codon.code [1] == 'r') {
						// nothing
					} else if (codon.code [1] == 'n') {
						speed += VARIATION_SPEED * 1;
					}
				} else if (codon.code [0] == 'n') {
					// technique stat
					if (codon.code [1] == 'p') {
						technique += VARIATION_TECHNIQUE * 1;
					} else if (codon.code [1] == 'e') {
						technique += VARIATION_TECHNIQUE * 2;
					} else if (codon.code [1] == 'r') {
						technique += VARIATION_TECHNIQUE * 3;
					} else if (codon.code [1] == 'n') {
						// nothing
					}
				}
			} else {
				// handle powers
			}
		}
	}
}

public class Codon {
	public bool isPower;
	public string code;

	public Codon (string code) {
		this.code = code;
		this.isPower = code.Contains ("x");
	}
}

[CreateAssetMenu(fileName = "Power", menuName = "NewPower", order = 1)]
public class Power : ScriptableObject {
	public string id;
	public List<string> codes;
	public string display;
	public string description;
}
