using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public int money;

	void Start() {
		money = 1000;
	}

	public void SpendMoney(int amount) {
		money -= amount;
	}
}
