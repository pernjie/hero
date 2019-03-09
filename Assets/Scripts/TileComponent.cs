using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileComponent : MonoBehaviour {
	public Tile tile;

	public void Initialise(Tile tile) {
		this.tile = tile;
		name = "Tile: (" + tile.x + ", " + tile.y + ")";
	}

	public void SetTileSprite(Sprite sprite) {
		GetComponentInChildren<SpriteRenderer> ().sprite = sprite;
	}

	public void SetCrimeOnTile() {
		GetComponentInChildren<SpriteRenderer> ().color = new Color(1f, .2f, .2f);
	}

	public void RemoveCrimeOnTile() {
		GetComponentInChildren<SpriteRenderer> ().color = new Color(1f, 1f, 1f);
	}
}

[System.Serializable]
public class Tile {
	public int x;
	public int y;
	public TileType tileType;

	public Tile(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public Tile(int x, int y, TileType tileType) {
		this.x = x;
		this.y = y;
		this.tileType = tileType;
	}

	public override int GetHashCode() {
		return ToString ().GetHashCode ();
	}

	public override string ToString () {
		return x + ", " + y;
	}

	public Tile ApplyDirection(Tile direction) {
		return new Tile(
			this.x + direction.x,
			this.y + direction.y
		);
	}
}

public enum TileType {
	Pavement,
	Road, 
	Building
}
