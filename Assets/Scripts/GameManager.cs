using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	LeftPanel leftPanel;

	int BASE_MONEY = 500;

	public int money;

	void Awake() {
		leftPanel = FindObjectOfType<LeftPanel> ();
	}

	void Start() {
		money = BASE_MONEY;
		updateMoney ();
	}

	public void SpendMoney(int amount) {
		money -= amount;
		updateMoney ();
	}

	void updateMoney() {
		leftPanel.UpdateMoneyUI (money);
	}
}
