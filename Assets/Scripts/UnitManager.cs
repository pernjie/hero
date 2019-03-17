using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
	City city;
	LeftPanel leftPanel;

	public GameObject HeroPrefab;
	public GameObject VillainPrefab;
	Dictionary<Unit, GameObject> UnitToGOMap;
	public List<Unit> units;

	void Awake() {
		city = FindObjectOfType<City> ();
		leftPanel = FindObjectOfType<LeftPanel> ();
	}

	public void Initialise() {
		UnitToGOMap = new Dictionary<Unit, GameObject> ();

		for (int i = 0; i < 4; i++) {
			AddHero ();
		}

		for (int i = 0; i < 2; i++) {
			AddVillain ();
		}
	}

	void AddHero() {
		GameObject heroGO = Instantiate (HeroPrefab);
		Hero newHero = new Hero (10, 3, 1f);
		heroGO.GetComponent<HeroMover> ().Initialise (newHero, city.GetRandomTile());
		units.Add (newHero);
		UnitToGOMap [newHero] = heroGO;
		leftPanel.AddPortrait (newHero);
	}

	void AddVillain() {
		GameObject villainGO = Instantiate (VillainPrefab);
		Villain newVillain = new Villain (2, 10, 4, 1f);
		units.Add (newVillain);
		villainGO.GetComponent<VillainMover> ().Initialise (newVillain, city.GetRandomTile());
		UnitToGOMap [newVillain] = villainGO;
		leftPanel.AddPortrait (newVillain);
	}

	public bool HandleUnitDamage(Unit attacker, Unit attacked) {
		int attackDamage = attacker.GetAttackDamage ();
		bool isKilled = attacked.DamageUnit (attackDamage);

		if (isKilled) {
			if (attacked.unitType == UnitType.Criminal)
				return true;

			// handle only hero and villain deaths
			GameObject unitGO = GetUnitGOFromUnit (attacked);
			if (unitGO != null) {
				unitGO.SendMessage ("KillUnit");
			}
		}

		leftPanel.UpdateHealth (attacked);

		return isKilled;
	}

	public GameObject GetUnitGOFromUnit(Unit unit) {
		try {
			return UnitToGOMap [unit];
		} catch (System.IndexOutOfRangeException) {
			return null;
		}
	}
}

[System.Serializable]
public class Unit {
	public string uid;
	public string name;
	public GeneticMaterial genetics;
	public Gender gender;
	public UnitType unitType;
	public bool isDead;
	public int maxHealth;
	public int currentHealth;

	public int attackDamage;
	public float nextAttackCounter;
	public float attackCooldown;

	public void InitialiseStats (UnitType unitType, int maxHealth, int attackDamage, float attackSpeed) {
		this.uid = "Unit " + Random.Range (0, 9999);
		this.gender = (Gender)Random.Range (0, 2);
		this.unitType = unitType;
		this.maxHealth = maxHealth;
		this.currentHealth = maxHealth;
		this.attackDamage = attackDamage;

		this.attackCooldown = 1f;
		this.nextAttackCounter = 0f;
	}

	public void HealUnit(int amount) {
		if (isDead)
			return;

		currentHealth += amount;
		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		}
	}

	// returns true if unit is killed
	public bool DamageUnit(int amount) {
		if (isDead)
			return false;

		currentHealth -= amount;
		if (currentHealth <= 0) {
			currentHealth = 0;
			isDead = true;
		}

		return isDead;
	}

	public int GetAttackDamage() {
		return attackDamage;
	}

	public override int GetHashCode() {
		return ToString ().GetHashCode ();
	}

	public override string ToString () {
		return uid;
	}

	public override bool Equals(System.Object obj) {
		if (obj == null)
			return false;

		Unit u = obj as Unit;
		return this.uid == u.uid;
	}

	public bool Equals(Unit u) {
		if ((object)u == null)
			return false;
		return this.uid == u.uid;
	}
}

[System.Serializable]
public class Hero : Unit {
	public Hero(int maxHealth, int attackDamage, float attackSpeed) {
		InitialiseStats (UnitType.Hero, maxHealth, attackDamage, attackSpeed);
		name = "Hero " + Random.Range (0, 9999);
	}
}


[System.Serializable]
public class Villain : Unit {
	public float planningCounter = 0f;
	public float planningTimeNeeded = 3f;

	public int leadership;

	public Villain(int leadership, int maxHealth, int attackDamage, float attackSpeed) {
		InitialiseStats (UnitType.Villain, maxHealth, attackDamage, attackSpeed);
		this.leadership = leadership;
		name = "Villain " + Random.Range (0, 9999);
	}
}

public enum UnitType {
	Hero,
	Villain,
	Criminal
}

public enum Gender {
	Male,
	Female
}
