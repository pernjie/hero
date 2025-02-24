﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMover : UnitMover {
	CrimeManager crimeManager;
	UnitManager unitManager;

	public Hero hero;
	public HeroState heroState;
	public Crime currentFocusedCrime;

	public void Initialise(Hero hero, Tile tile) {
		base.Initialise (tile, hero.movementDelay);
		crimeManager = FindObjectOfType<CrimeManager> ();
		unitManager = FindObjectOfType<UnitManager> ();

		this.hero = hero;
		heroState = HeroState.Patrol;
	}

	public void SetHeroState(HeroState newHeroState) {
		if (newHeroState == HeroState.ToCrime) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1f, .2f, .2f);
			delayBetweenTiles = (hero.movementDelay / 3f);
		} else if (newHeroState == HeroState.Patrol) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1f, 1f, 1f);
			delayBetweenTiles = hero.movementDelay;
		} else if (newHeroState == HeroState.Dead) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (0f, 0f, 0f);
		} else if (newHeroState == HeroState.Fleeing) {
			delayBetweenTiles = hero.movementDelay;
			GetComponentInChildren<SpriteRenderer> ().color = new Color (0f, 1f, 0f);
		}

		this.heroState = newHeroState;
	}

    // Update is called once per frame
    void Update() {
		MovementUpdate ();
		RegenerationUpdate ();

		// if patroling and not moving, find new path to patrol
		if ((heroState == HeroState.Patrol || heroState == HeroState.Fleeing) && !isMoving) {
			GoToTile (city.GetRandomTile ());
		} else if (heroState == HeroState.ToCrime) {
			if (!currentFocusedCrime.GetIsActive()) {
				currentFocusedCrime = null;
				SetHeroState(HeroState.Patrol);
				StopPath ();
			}
		} else if (heroState == HeroState.Fighting) {
			if (currentFocusedCrime.GetIsActive ()) {
				// pool to choose who to target first
				// TODO: consider AOE attacks?
				List<Criminal> targetableCriminals = currentFocusedCrime.GetTargetableCriminals();
				List<Villain> targetableVillains = currentFocusedCrime.GetTargetableVillains ();

				int criminalPool = targetableCriminals.Count * 1;
				int villainPool = targetableVillains.Count * 1;
				int totalPool = villainPool + criminalPool;

				if (totalPool > 0) {
					if (Random.Range (0, totalPool) < villainPool) {
						// select villain
						Villain targetVillain = targetableVillains[Random.Range(0, targetableVillains.Count)];

						// if attack is ready
						if (hero.nextAttackCounter > hero.attackCooldown) {
							unitManager.HandleUnitDamage (hero, targetVillain);
							hero.nextAttackCounter = 0f;
						} else {
							hero.nextAttackCounter += Time.deltaTime;
						}
					} else {
						// select criminal
						Criminal targetCriminal = targetableCriminals[Random.Range(0, targetableCriminals.Count)];

						// if attack is ready
						if (hero.nextAttackCounter > hero.attackCooldown) {
							unitManager.HandleUnitDamage (hero, targetCriminal);
							hero.nextAttackCounter = 0f;
						} else {
							hero.nextAttackCounter += Time.deltaTime;
						}
					}
				} else {
					// if nobody to attack, return
					return;
				}

			} else {
				// if crime not active, go back to patrol mode
				currentFocusedCrime = null;
				SetHeroState (HeroState.Patrol);
			}
		}
    }

	void RegenerationUpdate() {
		if (heroState == HeroState.Fleeing || heroState == HeroState.Patrol) {
			if (hero.regenerationCounter > 1f) {
				hero.regenerationCounter = 0f;
				unitManager.HandleUnitHeal (hero, hero);
			} else {
				hero.regenerationCounter += Time.deltaTime;
			}
		}
	}

	public void KillUnit() {
		Debug.Log (hero.name + " dead");
		SetHeroState (HeroState.Dead);
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

public enum HeroState {
	Patrol,
	ToCrime,
	Fighting,
	Dead,
	Fleeing
}
