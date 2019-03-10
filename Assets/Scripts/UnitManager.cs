using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
	City city;

	public GameObject HeroPrefab;
	public GameObject VillainPrefab;
	Dictionary<Unit, GameObject> UnitToGOMap;

	void Awake() {
		city = FindObjectOfType<City> ();
	}

	public void Initialise() {
		UnitToGOMap = new Dictionary<Unit, GameObject> ();

		for (int i = 0; i < 4; i++) {
			GameObject heroGO = Instantiate (HeroPrefab);
			heroGO.GetComponent<HeroMover> ().Initialise (city.GetRandomTile());
			UnitToGOMap [heroGO.GetComponent<HeroMover> ().hero] = heroGO;
		}

		for (int i = 0; i < 2; i++) {
			GameObject villainGO = Instantiate (VillainPrefab);
			villainGO.GetComponent<VillainMover> ().Initialise (city.GetRandomTile());
			UnitToGOMap [villainGO.GetComponent<VillainMover> ().villain] = villainGO;
		}
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
	public UnitType unitType;
	public bool isDead;
	public int maxHealth;
	public int currentHealth;

	public int attackDamage;
	public float nextAttackCounter;
	public float attackCooldown;

	public void InitialiseStats (UnitType unitType, int maxHealth, int attackDamage, float attackSpeed) {
		this.uid = "Unit " + Random.Range (0, 9999);
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
		if (currentHealth < 0) {
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

public enum UnitType {
	Hero,
	Villain,
	Criminal
}
