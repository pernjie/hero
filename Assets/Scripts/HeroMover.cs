using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMover : UnitMover {
	public Hero hero;
	CrimeManager crimeManager;
	public HeroState heroState;
	public Crime currentFocusedCrime;

	public new void Initialise(Tile tile) {
		base.Initialise (tile);
		heroState = HeroState.Patrol;
		crimeManager = FindObjectOfType<CrimeManager> ();
		hero = new Hero (10, 3, 1f);
	}

	void SetHeroState(HeroState newHeroState) {
		if (newHeroState == HeroState.ToCrime) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1f, .2f, .2f);
			delayBetweenTiles = 0.2f;
		} else if (newHeroState == HeroState.Patrol) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1f, 1f, 1f);
			delayBetweenTiles = 0.6f;
		}

		this.heroState = newHeroState;
	}

    // Update is called once per frame
    void Update() {
		MovementUpdate ();

		// if patroling and not moving, find new path to patrol
		if (heroState == HeroState.Patrol && !isMoving) {
			GoToTile (city.GetRandomTile ());
		} else if (heroState == HeroState.ToCrime) {
			if (!currentFocusedCrime.GetIsActive()) {
				currentFocusedCrime = null;
				SetHeroState(HeroState.Patrol);
				StopPath ();
			}
		} else if (heroState == HeroState.Fighting) {
			if (currentFocusedCrime.GetIsActive ()) {
				// TODO: handle fighting villains
			} else {
				// if crime not active, go back to patrol mode
				currentFocusedCrime = null;
				SetHeroState (HeroState.Patrol);
			}
		}
    }

	protected override void finishStepUnit () {
		if (heroState == HeroState.Patrol) {
			List<CrimeInRange> crimesInRange = crimeManager.FindCrimesInRange (currentTile, 3);
			if (crimesInRange.Count > 0) {
				// select crime in range to go to
				currentFocusedCrime = crimesInRange [0].crime;
				GoToTile (crimesInRange [0].closestTile);

				SetHeroState (HeroState.ToCrime);
			}
		}
	}

	protected override void finishPathUnit() {
		if (heroState == HeroState.ToCrime) {
			if (currentFocusedCrime.GetIsActive ()) {
				if (crimeManager.OnCrimeScene (currentTile, currentFocusedCrime.tiles)) {
					currentFocusedCrime.AddHeroToScene (hero);
					SetHeroState (HeroState.Fighting);
				} else {
					// if somehow finished on wrong tile, reroute to the actual crime tile
					GoToTile (crimeManager.GetCrimeInRange (currentTile, currentFocusedCrime.tiles).closestTile);
				}
			} else {
				// if crime already not active, go back to patrol
				currentFocusedCrime = null;
				SetHeroState (HeroState.Patrol);
				StopPath ();
			}
		}
	}
}

[System.Serializable]
public class Hero : Unit {
	public string name;
	public Hero(int maxHealth, int attackDamage, float attackSpeed) {
		InitialiseStats (maxHealth, attackDamage, attackSpeed);
		name = "Hero " + Random.Range (0, 9999);
	}
}

public enum HeroState {
	Patrol,
	ToCrime,
	Fighting
}
