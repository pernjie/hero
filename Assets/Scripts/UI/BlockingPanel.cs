using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingPanel : MonoBehaviour {
	LeftPanel leftPanel;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
	}

	public void OnClick() {
		leftPanel.ExitCurrentPopup ();
	}
}
