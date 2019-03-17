using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelViewButton : MonoBehaviour {
	LeftPanel leftPanel;
	public PanelView view;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
	}

	public void OnClick() {
		leftPanel.ToggleView (view);
	}
}
