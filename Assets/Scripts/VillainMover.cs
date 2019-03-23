using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillainMover : UnitMover {
	CrimeManager crimeManager;
	UnitManager unitManager;

	public Villain villain;
	public VillainState villainState;
	public Crime currentFocusedCrime;

	public new void Initialise(Villain villain, Tile tile) {
		base.Initialise (tile, villain.movementDelay);
		crimeManager = FindObjectOfType<CrimeManager> ();
		unitManager = FindObjectOfType<UnitManager> ();
		this.villain = villain;

		villainState = VillainState.Planning;
	}

	void SetVillainState(VillainState newVillainState) {
		if (newVillainState == VillainState.ToCrime) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (4f, .5f, .4f);
		} else if (newVillainState == VillainState.Planning) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1f, 1f, 1f);
		} else if (newVillainState == VillainState.Dead) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (0f, 0f, 0f);
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
				List<Hero> targetableHeroes = currentFocusedCrime.GetTargetableHeroes();
				if (targetableHeroes.Count > 0) {
					// if attack is ready
					if (villain.nextAttackCounter > villain.attackCooldown) {
						// select random hero?
						Hero heroToTarget = targetableHeroes [Random.Range (0, targetableHeroes.Count)];
						unitManager.HandleUnitDamage (villain, heroToTarget);

						villain.nextAttackCounter = 0f;
					} else {
						villain.nextAttackCounter += Time.deltaTime;
					}
				} else {
					// if no targetable heroes
					return;
				}
			} else {
				// plan new crime if crime is complete
				SetVillainState (VillainState.Planning);
			}
		}
    }

	public void KillUnit() {
		Debug.Log (villain.name + " dead");
		SetVillainState (VillainState.Dead);
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

public enum VillainState {
	Planning,
	ToCrime,
	Criming,
	Dead
}