using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GeneticIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	Transform breedingPopup;
	Transform geneticDock;
	public Text unitName;
	Vector3 startingPosition;

	bool inSlot;
	public Unit unit;

	void Awake() {
		breedingPopup = FindObjectOfType<BreedingPopup> ().gameObject.transform;
		geneticDock = FindObjectOfType<BreedingPopup> ().GeneticDock.transform;
	}

	void Start() {
		inSlot = false;
		startingPosition = this.transform.localPosition;
	}

	public void Initialise (Unit unit) {
		unitName.text = unit.name;
	}

	Transform startParent;
	Transform canvas;

	public void OnBeginDrag(PointerEventData eventData) {
		inSlot = false;

		transform.SetParent (breedingPopup);
		//GetComponentInChildren<Image> ().raycastTarget = false;
		GetComponent<CanvasGroup>().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		transform.SetAsLastSibling ();
		//transform.SetSiblingIndex(1);
	}

	public void OnDrag(PointerEventData eventData) {
		transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData) {
		resetPosition ();
	}

	public void DropOnSlot(BreedingSlot breedingSlot) {
		if (unit.gender == breedingSlot.genderSlot) {
			inSlot = true;
			Debug.Log ("DROP");
			this.transform.position = breedingSlot.gameObject.transform.position;
		} else {
			resetPosition ();
		}
	}

	void resetPosition() {
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		GetComponent<CanvasGroup> ().interactable = true;
		if (!inSlot) {
			transform.SetParent (geneticDock);
			this.transform.localPosition = startingPosition;
			//transform.SetAsLastSibling ();
		}
	}
}
