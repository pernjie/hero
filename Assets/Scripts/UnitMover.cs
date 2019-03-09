using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour {
	protected City city;

	public bool isMoving;
	public Tile currentTile;
	public List<Tile> currentPath;
	public int currentPathIndex;
	Vector3 newPosition;

	Vector3 currentVelocity;
	protected float delayBetweenTiles = 0.6f;

	void Awake () {
		city = FindObjectOfType<City> ();
	}

	public void Initialise(Tile tile) {
		currentTile = tile;
		transform.position = city.GetTilePosition (tile);
		newPosition = this.transform.position;
	}

	public void MovementUpdate () {
		if (currentTile == null)
			return;

		if (Vector2.Distance (transform.position, newPosition) > 0.05f) {
			this.transform.position = Vector3.SmoothDamp (this.transform.position, newPosition, ref currentVelocity, delayBetweenTiles);
		} else if (currentPath != null) {
			currentPathIndex += 1;
			if (currentPathIndex < currentPath.Count) {
				finishStepUnit ();
				currentTile = currentPath [currentPathIndex];
				newPosition = city.GetTilePosition(currentTile);
			} else {
				finishPath ();
			}
		}
	}

	public void finishPath() {
		finishStepUnit ();
		currentPath = null;
		isMoving = false;
		finishPathUnit ();
	}

	public void StopPath() {
		currentPath = null;
		isMoving = false;
	}

	protected virtual void finishStepUnit() { }
	protected virtual void finishPathUnit() { }

	public void GoToTile(Tile targetTile) {
		StopPath ();

		if (city.GetTileAt (targetTile) == null)
			return;

		List<Tile> path = city.AStarSearch (currentTile, targetTile);

		if (path.Count > 0)
			setPath (path);
	}

	void setPath(List<Tile> path) {
		isMoving = true;
		this.currentPathIndex = 0;
		this.currentPath = path;
		this.newPosition = city.GetTilePosition(currentTile);
	}
}
