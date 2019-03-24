using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {
	CrimeManager crimeManager;

	public GameObject tileDock;
	public GameObject tilePrefab;

	public GameObject housePrefab;

	public Sprite[] tileSprites;

	int MAP_WIDTH = 7;
	int MAP_HEIGHT = 7;
	float TILE_WIDTH = 1.9f;
	float TILE_HEIGHT = .95f;
	float MAP_OFFSET_X = 1.3f;
	float MAP_OFFSET_Y = 0f;

	Tile[] neighbourDirections = new Tile[] {
		new Tile(+1, 0), // up right
		new Tile(-1, 0), // down left
		new Tile(0, -1), // up left
		new Tile(0, +1)  // down right
	};

	//Tile[,] tiles;
	int mapRowStart, mapRowEnd, mapColStart, mapColEnd;
	Dictionary<string, Tile> tiles;
	public List<Tile> tileList;
	Dictionary<Tile, GameObject> tileToGOMap;

	void Awake() {
		crimeManager = FindObjectOfType<CrimeManager> ();
	}

    // Start is called before the first frame update
    void Start() {
		GenerateMap ();
		SetTileSprites ();

		GameObject houseGO = Instantiate (housePrefab, tileDock.transform);
		houseGO.transform.position = GetTilePosition (2, 4);

		FindObjectOfType<UnitManager> ().Initialise ();
    }

	void GenerateMap() {
		// setup total map size in terms of pixels
		float MAP_TOTAL_WIDTH = MAP_WIDTH * TILE_WIDTH;
		// float MAP_TOTAL_HEIGHT = MAP_HEIGHT * TILE_HEIGHT;

		// initialise stuff
		tileList = new List<Tile> ();
		//tiles = new Tile[MAP_WIDTH, MAP_HEIGHT];
		tiles = new Dictionary<string, Tile> ();
		tileToGOMap = new Dictionary<Tile, GameObject> ();

		for (int i = 0; i < MAP_WIDTH; i++) {
			for (int j = 0; j < MAP_HEIGHT; j++) {
				GameObject tileGO = Instantiate (tilePrefab, tileDock.transform);
				tileGO.transform.localPosition = new Vector2 (
					MAP_OFFSET_X - (MAP_TOTAL_WIDTH /2f) + (j * TILE_WIDTH/2f) + (i * TILE_WIDTH/2f) + (TILE_WIDTH/2f), 
					MAP_OFFSET_Y + (i * TILE_HEIGHT/2f) - (j * TILE_HEIGHT/2f)
				);

				TileType tileType;
				// set terrain
				if (i == 1 || i == 3 || j == 1 || j == 5) {
					tileType = TileType.Road;
				} else {
					tileType = TileType.Pavement;
				}

				if (i == 2 && j == 4)
					tileType = TileType.Building;

				Tile tile = new Tile (i, j, tileType);
				tileGO.GetComponent<TileComponent> ().Initialise(tile);

				tileToGOMap [tile] = tileGO;
				//tiles [tile.x, tile.y] = tile;
				tiles[tile.ToString()] = tile;
				tileList.Add (tile);
			}
		}

		mapRowStart = 0;
		mapRowEnd = MAP_WIDTH - 1;
		mapColStart = 0;
		mapColEnd = MAP_HEIGHT - 1;
	}

	// expand one row and one column in both directions
	public void ExpandCity() {
		float MAP_TOTAL_WIDTH = MAP_WIDTH * TILE_WIDTH;

		for (int i = mapRowStart - 1; i < mapRowEnd + 2; i++) {
			for (int j = mapColStart - 1; j < mapColEnd + 2; j++) {
				if (i >= mapRowStart && i <= mapRowEnd && j >= mapColStart && j <= mapColEnd)
					continue;

				Debug.Log (i + ", " + j);

				GameObject tileGO = Instantiate (tilePrefab, tileDock.transform);
				tileGO.transform.localPosition = new Vector2 (
					MAP_OFFSET_X - (MAP_TOTAL_WIDTH / 2f) + (j * TILE_WIDTH / 2f) + (i * TILE_WIDTH / 2f) + (TILE_WIDTH / 2f), 
					MAP_OFFSET_Y + (i * TILE_HEIGHT / 2f) - (j * TILE_HEIGHT / 2f)
				);

				TileType tileType;
				// set terrain
				if (i == 1 || i == 3 || j == 1 || j == 5) {
					tileType = TileType.Road;
				} else {
					tileType = TileType.Pavement;
				}

				if (i == 2 && j == 4)
					tileType = TileType.Building;

				Tile tile = new Tile (i, j, tileType);
				tileGO.GetComponent<TileComponent> ().Initialise (tile);

				tileToGOMap [tile] = tileGO;
				//tiles [tile.x, tile.y] = tile;
				tiles [tile.ToString ()] = tile;
				tileList.Add (tile);
			}
		}

		mapRowStart -= 1;
		mapRowEnd += 1;
		mapColStart -= 1;
		mapColEnd += 1;

		// set tile sprites for all neighbours
		SetTileSprites();
	}

	public GameObject GetTileGOAt(int x, int y) {
		//Tile t = GetTileAt (new Tile(x, y));
		try {
			Tile t = tiles[x + ", " + y];
			return GetTileGOFromTile(t);
		} catch (KeyNotFoundException) {
			return null;
		}
	}

	public GameObject GetTileGOFromTile(Tile t) {
		if (t != null)
			return tileToGOMap [t];
		else
			return null;
	}

	// dont return tile with crime
	public Tile GetRandomTile() {
		List<Tile> shuffled = shuffleList<Tile> (tileList);

		for (int i = 0; i < shuffled.Count; i++) {
			if (!crimeManager.CheckTileHasCrime(shuffled[i]))
				return shuffled[i];
		}
		return null;
	}

	public List<Tile> shuffleList<T>(List<Tile> theList) {
		for (int i=0;i<theList.Count;i++) {
			Tile temp = theList[i];
			int randomIndex = Random.Range (i, theList.Count);
			theList [i] = theList [randomIndex];
			theList [randomIndex] = temp;
		}

		return theList;
	}

	public void SetTileSprites() {
		foreach (Tile tile in tileList) {
			if (tile.tileType == TileType.Road) {
				Tile upright = GetTileAt (tile.ApplyDirection (neighbourDirections [0]));
				Tile downleft = GetTileAt (tile.ApplyDirection (neighbourDirections [1]));
				Tile upleft = GetTileAt (tile.ApplyDirection (neighbourDirections [2]));
				Tile downright = GetTileAt (tile.ApplyDirection (neighbourDirections [3]));

				if (upright == null || upright.tileType == TileType.Road) {
					if (downleft == null || downleft.tileType == TileType.Road) {
						if (upleft == null || upleft.tileType == TileType.Road) {
							GetTileGOFromTile (tile).GetComponent<TileComponent> ().SetTileSprite (tileSprites [3]);
						} else {
							GetTileGOFromTile (tile).GetComponent<TileComponent> ().SetTileSprite (tileSprites [2]);
						}
					}
				} else if (upleft == null || upleft.tileType == TileType.Road) {
					if (downright == null || downright.tileType == TileType.Road) {
						GetTileGOFromTile (tile).GetComponent<TileComponent> ().SetTileSprite (tileSprites [1]);
					}
				}
			}
		}
	}

	public int GetDistance(Tile tile1, Tile tile2) {
		return Mathf.Abs(tile1.x - tile2.x) + Mathf.Abs(tile1.y - tile2.y);
	}

	public Tile GetTileAt(Tile tile) {
		try {
			return tiles[tile.x + ", " + tile.y];
		} catch (KeyNotFoundException) {
			return null;
		}
	}

	public Vector3 GetTilePosition(int x, int y) {
		return GetTilePosition (new Tile (x, y));
	}

	public Vector3 GetTilePosition(Tile tile) {
		Vector3 tilePos = tileToGOMap [GetTileAt(tile)].transform.position;
		return new Vector3 (tilePos.x, tilePos.y, -1f);
	}

	int GetCostOfTravel(Tile tile1, Tile tile2) {
		if (tile1.tileType == TileType.Pavement) {
			if (tile2.tileType == TileType.Pavement) {
				return 2;
			} else if (tile2.tileType == TileType.Road) {
				return 2;
			}
		} else if (tile1.tileType == TileType.Road) {
			if (tile2.tileType == TileType.Pavement) {
				return 2;
			} else if (tile2.tileType == TileType.Road) {
				return 1;
			}
		}
		return 5;
	}

	public List<Tile> AStarSearch (Tile startTile, Tile endTile) {
		Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
		Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();

		PriorityQueue frontier = new PriorityQueue();
		frontier.Enqueue(startTile, 0);

		cameFrom[startTile] = startTile;
		costSoFar[startTile] = 0;

		int n = 0;
		while (frontier.Count > 0) {
			n += 1;
			if (n > 1000) {
				break;
			}

			Tile current = frontier.Dequeue ();
			if (current.Equals (endTile)) {
				break;
			}

			foreach (Tile next in GetNeighbours(current)) {
				// to also check traversing particular terrain cost
				int newCost = costSoFar [current] + GetCostOfTravel(current, next);
				if (!costSoFar.ContainsKey (next) || newCost < costSoFar [next]) {
					costSoFar [next] = newCost;
					double priority = newCost + GetCostOfTravel (next, endTile);
					frontier.Enqueue (next, priority);
					cameFrom [next] = current;
				}
			}
		}

		List<Tile> path = new List<Tile> ();
		Tile traverse = endTile;

		while (!traverse.Equals(startTile)) {
			path.Add (traverse);
			try {
				traverse = cameFrom [traverse];
			} catch (KeyNotFoundException) {
				//Debug.Log ("incomplete path");
				return new List<Tile> ();
			}
		}

		path.Add (startTile);
		path.Reverse ();
		return path;
	}

	public List<Tile> GetNeighbours(Tile tile) {
		List<Tile> neighbours = new List<Tile> ();

		foreach (Tile neighbourDirection in neighbourDirections) {
			Tile neighbourTile = GetTileAt (tile.ApplyDirection (neighbourDirection));
			if (neighbourTile != null) {
				neighbours.Add (neighbourTile);
			}
		}

		return neighbours;
	}

	class Tuple {
		public Tile t;
		public double d;

		public Tuple(Tile t, double d) {
			this.t = t;
			this.d = d;
		}
	}

	public class PriorityQueue {
		private List<Tuple> elements = new List<Tuple>();

		public int Count {
			get { return elements.Count; }
		}

		public void Enqueue(Tile item, double priority) {
			elements.Add (new Tuple (item, priority));
		}

		public Tile Dequeue() {
			int bestIndex = 0;

			for (int i = 0; i < elements.Count; i++) {
				if (elements [i].d < elements [bestIndex].d)
					bestIndex = i;
			}

			Tile bestItem = elements [bestIndex].t;
			elements.RemoveAt (bestIndex);
			return bestItem;
		}
	}
}
