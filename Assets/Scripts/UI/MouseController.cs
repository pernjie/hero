using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {
	City city;

	public GameObject unitDummy;
	public LayerMask layerIDForTiles;

	public MouseMode mouseMode;
	public delegate void UpdateFunc();
	UpdateFunc Update_CurrentFunc;
	public delegate void TargetingCallbackFunc(Tile tile);
	TargetingCallbackFunc targetingCallbackFunc;

	void Awake() {
		city = FindObjectOfType<City> ();
	}

	void Start() {
		Update_CurrentFunc = Update_ViewTileDetails;
		mouseMode = MouseMode.Default;

		//ChangeMouseMode (MouseMode.Targeting);
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
		if (tile != null) {
			if (unitDummy.transform.localScale == new Vector3 (0f, 0f, 0f)) {
				ShowUnitDummy ();
			}

			Vector3 tilePosition = city.GetTileGOFromTile (tile).transform.position;
			unitDummy.transform.position = new Vector3 (tilePosition.x, tilePosition.y, -1f);

			// if user clicks, then current tile is selected as target (note this is in the if block that we can assume tile exists)
			if (Input.GetMouseButtonDown (0)) {
				targetingCallbackFunc (tile);
				HideUnitDummy ();
				ChangeMouseMode (MouseMode.Default);
			}
		}
	}

	public void ChangeMouseMode(MouseMode newMode, TargetingCallbackFunc targetingCallbackFunc=null) {
		if (newMode == MouseMode.Default) {
			Update_CurrentFunc = Update_ViewTileDetails;
		} else if (newMode == MouseMode.Targeting) {
			Update_CurrentFunc = Update_TargetTile;
			this.targetingCallbackFunc = targetingCallbackFunc;
		}

		this.mouseMode = newMode;
	}

	void ShowUnitDummy() {
		unitDummy.transform.localScale = new Vector3 (1f, 1f, 1f);
	}

	void HideUnitDummy() {
		unitDummy.transform.localScale = new Vector3 (0f, 0f, 0f);
	}

	Tile GetTileOnCursor() {
		Vector2 worldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		RaycastHit2D hitInfo = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layerIDForTiles);
		if (hitInfo.collider != null) {
			Tile tile = hitInfo.collider.gameObject.GetComponent<TileComponent> ().tile;
			return tile;
		}
		return null;
	}
}

public enum MouseMode {
	Default,
	Targeting
}
