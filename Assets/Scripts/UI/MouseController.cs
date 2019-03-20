using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {
	City city;

	public delegate void UpdateFunc();
	UpdateFunc Update_CurrentFunc;

	public LayerMask LayerIDForTiles;

	void Awake() {
		city = FindObjectOfType<City> ();
	}

	void Start() {
		Update_CurrentFunc = Update_ViewTileDetails;
	}

    // Update is called once per frame
    void Update() {
		if (!EventSystem.current.IsPointerOverGameObject ()) {
			Update_CurrentFunc ();
		} else {
			// mouse over UI, dont do game stuff

		}
    }

	void Update_ViewTileDetails() {
		//Tile tile = GetTileOnCursor ();
	}

	void Update_TargetTile() {
		Tile tile = GetTileOnCursor ();
	}

	Tile GetTileOnCursor() {
		Vector2 worldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		RaycastHit2D hitInfo = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, LayerIDForTiles);
		if (hitInfo.collider != null) {
			Tile tile = hitInfo.collider.gameObject.GetComponent<TileComponent> ().tile;
			return tile;
		}
		return null;
	}
}
