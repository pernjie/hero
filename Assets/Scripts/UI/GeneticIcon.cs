using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GeneticIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	BreedingPopup breedingPopup;
	Transform breedingPopupTransform;
	Transform geneticDockTransform;
	public Text unitName;
	Vector3 startingPosition;

	bool inSlot;
	public Unit unit;

	void Awake() {
		breedingPopup = FindObjectOfType<BreedingPopup> ();
		breedingPopupTransform = FindObjectOfType<BreedingPopup> ().gameObject.transform;
		geneticDockTransform = FindObjectOfType<BreedingPopup> ().GeneticDock.transform;
	}

	void Start() {
		inSlot = false;
		startingPosition = this.transform.localPosition;
	}

	public void Initialise (Unit unit) {
		this.unit = unit;
		unitName.text = unit.name;
	}

	Transform startParent;
	Transform canvas;

	public void OnBeginDrag(PointerEventData eventData) {
		inSlot = false;

		transform.SetParent (breedingPopupTransform);
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
		ResetPosition ();
	}

	public void DropOnSlot(BreedingSlot breedingSlot) {
		if (unit.gender == breedingSlot.genderSlot) {
			inSlot = true;
			this.transform.position = breedingSlot.gameObject.transform.position;
			breedingPopup.AddIconToSlot (this, breedingSlot);
		} else {
			ResetPosition ();
		}
	}

	public void ResetPosition() {
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		GetComponent<CanvasGroup> ().interactable = true;
		if (!inSlot) {
			breedingPopup.RemoveIconFromSlot (this);
			transform.SetParent (geneticDockTransform);
			this.transform.localPosition = startingPosition;
			//transform.SetAsLastSibling ();
		}
	}
}
