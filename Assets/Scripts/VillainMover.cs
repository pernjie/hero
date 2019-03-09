using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillainMover : UnitMover {
	public Villain villain;
	CrimeManager crimeManager;
	public VillainState villainState;
	public Crime currentFocusedCrime;

	public new void Initialise(Tile tile) {
		base.Initialise (tile);
		villainState = VillainState.Planning;
		crimeManager = FindObjectOfType<CrimeManager> ();
		this.villain = new Villain (2, 10, 4, 1f);
	}

	void SetVillainState(VillainState newVillainState) {
		if (newVillainState == VillainState.ToCrime) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1f, .2f, .2f);
		} else if (newVillainState == VillainState.Planning) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1f, 1f, 1f);
		}

		this.villainState = newVillainState;
	}

    // Update is called once per frame
    void Update() {
		MovementUpdate ();

		if (villainState == VillainState.Planning && !isMoving) {
			GoToTile (city.GetRandomTile ());
		} else if (villainState == VillainState.Planning) {
			// if planning is complete, create new crime to travel towards and start
			if (villain.planningCounter > villain.planningTimeNeeded) {
				villain.planningCounter = 0f;

				Crime newCrime = crimeManager.SpawnNewVillainCrime (villain);
				currentFocusedCrime = newCrime;

				GoToTile (crimeManager.GetCrimeInRange (currentTile, newCrime.tiles).closestTile);
				SetVillainState (VillainState.ToCrime);
			} else {
				// progress planning counter if still planning
				villain.planningCounter += Time.deltaTime;
			}
		} else if (villainState == VillainState.Criming) {
			if (!currentFocusedCrime.crimeComplete) {
				// if got hero on scene, handle fighting
				if (currentFocusedCrime.heroes != null && currentFocusedCrime.heroes.Count > 0) {
					// if attack is ready
					if (villain.nextAttackCounter > villain.attackCooldown) {
						// select random hero?
						Hero heroToTarget = currentFocusedCrime.heroes[Random.Range(0, currentFocusedCrime.heroes.Count)];
						heroToTarget.DamageUnit (villain.GetAttackDamage ());

						villain.nextAttackCounter = 0f;
					} else {
						villain.nextAttackCounter += Time.deltaTime;
					}
				}
			} else {
				// plan new crime if crime is complete
				SetVillainState (VillainState.Planning);
			}
		}
    }

	protected override void finishPathUnit () {
		if (villainState == VillainState.ToCrime) {
			if (crimeManager.OnCrimeScene (currentTile, currentFocusedCrime.tiles)) {
				if (currentFocusedCrime != null && !currentFocusedCrime.crimeStarted) {
					crimeManager.StartCrime (currentFocusedCrime);
				}

				SetVillainState (VillainState.Criming);
			} else {
				// if somehow finished on wrong tile, reroute to the actual crime tile
				GoToTile (crimeManager.GetCrimeInRange (currentTile, currentFocusedCrime.tiles).closestTile);
			}
		}
	}
}

[System.Serializable]
public class Villain : Unit {
	public float planningCounter = 0f;
	public float planningTimeNeeded = 3f;

	public string name;
	public int leadership;

	public Villain(int leadership, int maxHealth, int attackDamage, float attackSpeed) {
		InitialiseStats (maxHealth, attackDamage, attackSpeed);
		this.leadership = leadership;
		name = "Villain " + Random.Range (0, 9999);
	}
}

public enum VillainState {
	Planning,
	ToCrime,
	Criming
}