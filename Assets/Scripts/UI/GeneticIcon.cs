using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GeneticIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	LeftPanel leftPanel;
	BreedingPopup breedingPopup;
	Transform breedingPopupTransform;
	Transform geneticDockTransform;
	public Text unitName;
	Vector3 startingPosition;

	bool inSlot;
	public GeneticSample sample;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
		breedingPopup = FindObjectOfType<BreedingPopup> ();
		breedingPopupTransform = FindObjectOfType<BreedingPopup> ().gameObject.transform;
		geneticDockTransform = FindObjectOfType<BreedingPopup> ().geneticDock.transform;
	}

	void Start() {
		inSlot = false;
		startingPosition = this.transform.localPosition;
	}

	public void Initialise (GeneticSample sample) {
		this.sample = sample;
		unitName.text = sample.unit.name;

		if (sample.unit.gender == Gender.Male) {
			unitName.color = new Color (.2f, .2f, 1f);
		} else {
			unitName.color = new Color (1f, .2f, .2f);
		}
	}

	Transform startParent;
	Transform canvas;

	public void OnBeginDrag(PointerEventData eventData) {
		inSlot = false;

		transform.SetParent (breedingPopupTransform);

		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		transform.SetAsLastSibling ();
	}

	public void OnDrag(PointerEventData eventData) {
		transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData) {
		ResetPosition ();
	}

	public void OnMouseOver() {
		// view tooltip of unit details
		leftPanel.ShowUnitDetailsPopup(sample.unit, false);
	}

	public void OnMouseOff() {
		leftPanel.HideUnitDetailsPopup ();
	}

	public void DropOnSlot(BreedingSlot breedingSlot) {
		if (sample.unit.gender == breedingSlot.genderSlot) {
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
		}
	}
}
