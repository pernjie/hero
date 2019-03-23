using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimeManager : MonoBehaviour {
	City city;
	UnitManager unitManager;
	float crimeSpawnCounter = 5f;
	float crimeSpawnCooldown;

	public List<Crime> currentCrimes;

	void Awake() {
		city = FindObjectOfType<City> ();
		unitManager = FindObjectOfType<UnitManager> ();
		currentCrimes = new List<Crime> ();
		crimesToExpire = new List<Crime> ();
	}

    // Start is called before the first frame update
    void Start() {
		crimeSpawnCooldown = 5f;
    }

	List<Crime> crimesToExpire;
    // Update is called once per frame
    void Update() {
		if (crimeSpawnCounter > crimeSpawnCooldown) {
			SpawnNewPettyCrime ();
			crimeSpawnCounter = 0f;
		} else {
			crimeSpawnCounter += Time.deltaTime;
		}

		// progress running crimes
		foreach (Crime crime in currentCrimes) {
			if (crime.GetIsActive ()) {
				// progress crime if crime is still active
				crime.crimeCurrentProgress += Time.deltaTime;

				// check if crime is completed
				if (crime.crimeCurrentProgress > crime.crimeProgressNeeded) {
					city.GetTileGOFromTile (crime.tiles[0]).GetComponent<TileComponent> ().HideTime();

					foreach (Tile tile in crime.tiles) {
						crime.crimeComplete = true;
						city.GetTileGOFromTile (tile).GetComponent<TileComponent> ().RemoveCrimeOnTile ();

						crimesToExpire.Add (crime);
					}
				} else {
					city.GetTileGOFromTile (crime.tiles[0]).GetComponent<TileComponent> ().DisplayTime (crime.crimeProgressNeeded - crime.crimeCurrentProgress);

					// if got hero on scene, handle fighting
					List<Hero> targetableHeroes = crime.GetTargetableHeroes();
					if (targetableHeroes.Count > 0) {
						foreach (Criminal criminal in crime.criminals) {
							// if attack is ready
							if (criminal.nextAttackCounter > criminal.attackCooldown) {
								// select random hero?
								Hero heroToTarget = targetableHeroes [Random.Range (0, targetableHeroes.Count)];
								unitManager.HandleUnitDamage (criminal, heroToTarget);

								criminal.nextAttackCounter = 0f;
							} else {
								criminal.nextAttackCounter += Time.deltaTime;
							}
						}
					} else {
						// if no targetable heroes
						continue;
					}
				}
			}
		}

		if (crimesToExpire.Count > 0) {
			foreach (Crime crime in crimesToExpire) {
				currentCrimes.Remove (crime);
			}
			crimesToExpire.Clear ();
		}
    }

	public Crime SpawnNewPettyCrime() {
		// TODO: configure criminal stats
		List<Criminal> criminals = new List<Criminal> () { new Criminal(UnitType.Criminal, 2, 1, 5) };
		Crime newCrime = new Crime (CrimeType.Robbery, new Tile[] { city.GetRandomTile () }, criminals, null, 2f);
		currentCrimes.Add (newCrime);

		StartCrime (newCrime);

		return newCrime;
	}

	public Crime SpawnNewVillainCrime(Villain villain) {
		List<Criminal> criminals = new List<Criminal> ();
		for (int i = 0; i < villain.leadership; i++) {
			criminals.Add (new Criminal (UnitType.Criminal, 2, 1, 5));
		}

		Crime newCrime = new Crime (CrimeType.Robbery, new Tile[] { city.GetRandomTile () }, criminals, new List<Villain>() {villain}, 4f);
		currentCrimes.Add (newCrime);

		return newCrime;
	}

	public void StartCrime(Crime crime) {
		crime.crimeStarted = true;
		foreach (Tile tile in crime.tiles) {
			city.GetTileGOFromTile (tile).GetComponent<TileComponent> ().SetCrimeOnTile ();
		}
	}

	public List<CrimeInRange> FindCrimesInRange(Tile baseTile, int range) {
		List<CrimeInRange> crimesInRange = new List<CrimeInRange> ();
		foreach (Crime crime in currentCrimes) {
			if (!crime.GetIsActive())
				continue;

			CrimeInRange crimeInRange = GetCrimeInRange (baseTile, crime.tiles);
			if (crimeInRange.distance <= range) {
				crimeInRange.crime = crime;
				crimesInRange.Add (crimeInRange);
			}
		}

		return crimesInRange;
	}

	public CrimeInRange GetCrimeInRange(Tile baseTile, Tile[] crimeTiles) {
		Tile closestCrimeTile = null;
		int closestCrimeTileDistance = -1;
		foreach (Tile crimeTile in crimeTiles) {
			int crimeDistance = city.GetDistance (baseTile, crimeTile);
			if (closestCrimeTile == null || crimeDistance < closestCrimeTileDistance) {
				closestCrimeTile = crimeTile;
				closestCrimeTileDistance = crimeDistance;
			}
		}

		return new CrimeInRange (null, closestCrimeTileDistance, closestCrimeTile);
	}

	public bool CheckTileHasCrime(Tile currentTile) {
		foreach (Crime crime in currentCrimes) {
			foreach (Tile crimeTile in crime.tiles) {
				if (crimeTile == currentTile)
					return true;
			}
		}

		return false;
	}

	public bool OnCrimeScene(Tile currentTile, Tile[] crimeTiles) {
		foreach (Tile crimeTile in crimeTiles) {
			if (currentTile == crimeTile) {
				return true;
			}
		}

		return false;
	}
}

[System.Serializable]
public class Criminal : Unit {
	
	public Criminal(UnitType unitType, int health, int strength, int speed) {
		Initialise (unitType, health, strength, speed);
	}

}

[System.Serializable]
public class Crime {
	public CrimeType crimeType;
	public List<Criminal> criminals;
	public List<Villain> villains;
	public List<Hero> heroes;
	public Tile[] tiles;
	public bool crimeStarted;
	public bool crimeComplete;
	public float crimeProgressNeeded;
	public float crimeCurrentProgress;

	public Crime(CrimeType crimeType, Tile [] tiles, List<Criminal> criminals, List<Villain> villains, float crimeProgressNeeded) {
		this.crimeType = crimeType;
		this.criminals = criminals;
		this.villains = villains;
		this.tiles = tiles;
		this.crimeProgressNeeded = crimeProgressNeeded;
		crimeStarted = false;
		crimeComplete = false;
		crimeCurrentProgress = 0f;
	}

	public void AddHeroToScene(Hero hero) {
		if (heroes == null) {
			this.heroes = new List<Hero> ();
		}

		heroes.Add (hero);
	}

	public List<Hero> GetTargetableHeroes() {
		List<Hero> targetableHeroes = new List<Hero> ();

		// if empty
		if (heroes == null)
			return targetableHeroes;

		foreach (Hero hero in heroes) {
			if (!hero.isDead)
				targetableHeroes.Add (hero);
		}
		return targetableHeroes;
	}

	public List<Criminal> GetTargetableCriminals() {
		List<Criminal> targetableCriminals = new List<Criminal> ();

		// if empty
		if (criminals == null)
			return targetableCriminals;

		foreach (Criminal criminal in criminals) {
			if (!criminal.isDead)
				targetableCriminals.Add (criminal);
		}
		return targetableCriminals;
	}

	public List<Villain> GetTargetableVillains() {
		List<Villain> targetableVillains = new List<Villain> ();

		// if empty
		if (villains == null)
			return targetableVillains;

		foreach (Villain villain in villains) {
			if (!villain.isDead)
				targetableVillains.Add (villain);
		}
		return targetableVillains;
	}

	public bool GetIsActive() {
		return (crimeStarted && !crimeComplete);
	}
}

public class CrimeInRange {
	public Crime crime;
	public int distance;
	public Tile closestTile;

	public CrimeInRange(Crime crime, int distance, Tile closestTile) {
		this.crime = crime;
		this.distance = distance;
		this.closestTile = closestTile;
	}
}

public enum CrimeType {
	Robbery
}
