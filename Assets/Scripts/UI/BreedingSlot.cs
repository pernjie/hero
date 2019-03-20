using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BreedingSlot : MonoBehaviour, IDropHandler {
	public Gender genderSlot;
	public GeneticIcon iconInSlot;

	public void SetIconInSlot(GeneticIcon icon) {
		iconInSlot = icon;
	}

	public void RemoveIconFromSlot() {
		iconInSlot = null;
	}

	public void OnDrop(PointerEventData eventData) {
		eventData.pointerDrag.GetComponent<GeneticIcon> ().DropOnSlot (this);
	}
}
