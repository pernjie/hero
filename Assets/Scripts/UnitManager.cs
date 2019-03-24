using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
	City city;
	LeftPanel leftPanel;
	BreedingManager breedingManager;
	CrimeManager crimeManager;

	public GameObject heroPrefab;
	public GameObject villainPrefab;
	Dictionary<Unit, GameObject> unitToGOMap;
	public List<Unit> units;

	void Awake() {
		city = FindObjectOfType<City> ();
		leftPanel = FindObjectOfType<LeftPanel> ();
		breedingManager = FindObjectOfType<BreedingManager> ();
		crimeManager = FindObjectOfType<CrimeManager> ();
	}

	public void Initialise() {
		unitToGOMap = new Dictionary<Unit, GameObject> ();

		for (int i = 0; i < 3; i++) {
			AddHero (breedingManager.GetNewHero(), city.GetRandomTile());
		}

		for (int i = 0; i < 2; i++) {
			AddVillain (breedingManager.GetNewVillain (), city.GetRandomTile());
		}

		// FOR TESTING
		foreach (Unit unit in units) {
			BreedingManager breedingManager = FindObjectOfType<BreedingManager> ();
			breedingManager.AddGeneticMaterial (unit);
		}
	}

	public void AddHero(Hero hero, Tile tile) {
		GameObject heroGO = Instantiate (heroPrefab);

		heroGO.GetComponent<HeroMover> ().Initialise (hero, tile);
		units.Add (hero);
		unitToGOMap [hero] = heroGO;
		leftPanel.AddPortrait (hero);
	}

	public void AddVillain(Villain villain, Tile tile) {
		GameObject villainGO = Instantiate (villainPrefab);

		units.Add (villain);
		villainGO.GetComponent<VillainMover> ().Initialise (villain, tile);
		unitToGOMap [villain] = villainGO;
		leftPanel.AddPortrait (villain);
	}

	public bool HandleUnitDamage(Unit attacker, Unit attacked) {
		int attackDamage = attacker.GetAttackDamage ();
		bool isKilled = attacked.DamageUnit (attackDamage);

		if (isKilled) {
			if (attacked.unitType == UnitType.Criminal) {
				return true;
			}

			// handle only hero and villain deaths
			GameObject unitGO = GetUnitGOFromUnit (attacked);
			if (unitGO != null) {
				unitGO.SendMessage ("KillUnit");
			}

			// to remove hero from scene
			if (attacked.unitType == UnitType.Hero) {
				crimeManager.RemoveHeroFromScene (attacked);
			} else if (attacked.unitType == UnitType.Villain) {
				crimeManager.RemoveVillainFromScene (attacked);
			}
		} else if (attacked.currentHealth < attacked.fleeHealth) {
			// check if health below unit flee threshold. if so, attempt to flee
			// TODO: fleeing fail
			if (attacked.unitType == UnitType.Hero) {
				crimeManager.HandleHeroFlee (attacked);
			} else if (attacked.unitType == UnitType.Villain) {
				crimeManager.HandleVillainFlee (attacked);
			}
		}

		leftPanel.UpdateHealth (attacked);

		return isKilled;
	}

	public void HandleUnitHeal(Unit healer, Unit healed) {
		healed.HealUnit (healed.genetics.regenerationRate);

		leftPanel.UpdateHealth (healed);
	}

	public GameObject GetUnitGOFromUnit(Unit unit) {
		try {
			return unitToGOMap [unit];
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

	public float movementDelay;

	public int fleeHealth;

	public float recoveryCooldown;
	public float recoveryCounter;
	public float regenerationCounter;

	public Unit() { }

	public void Initialise (GeneticMaterial genetics) {
		this.genetics = genetics;

		this.uid = "Unit " + Random.Range (0, 9999);
		this.gender = (Gender)Random.Range (0, 2);
		this.unitType = UnitType.None; // temporary

		this.maxHealth = genetics.health;
		this.currentHealth = maxHealth;
		this.attackDamage = genetics.strength;

		this.attackCooldown = 15f/(float)genetics.speed;
		this.nextAttackCounter = 0f;

		this.movementDelay = 10f / (float)genetics.speed;

		this.fleeHealth = (int) ((float)maxHealth * genetics.fleeThreshold);
	}

	// for criminals
	public void Initialise (UnitType unitType, int health, int strength, int speed) {
		this.uid = "Unit " + Random.Range (0, 9999);
		this.gender = (Gender)Random.Range (0, 2);
		this.unitType = unitType;
		this.maxHealth = health;
		this.currentHealth = maxHealth;
		this.attackDamage = strength;

		this.attackCooldown = 15f/(float)speed;
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

	public int GetCollectCost() {
		return 100;
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
	
	public Hero(string name) {
		this.name = name;
	}
}


[System.Serializable]
public class Villain : Unit {
	public float planningCounter = 0f;
	public float planningTimeNeeded = 3f;

	public int leadership;

	public Villain(string name) {
		this.name = name;
		// TODO
		this.leadership = 5;
	}
}

public enum UnitType {
	None,
	Hero,
	Villain,
	Criminal
}

public enum Gender {
	Male,
	Female
}
