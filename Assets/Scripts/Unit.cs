[System.Serializable]
public class Unit {
	public bool isDead;
	public int maxHealth;
	public int currentHealth;

	public int attackDamage;
	public float nextAttackCounter;
	public float attackCooldown;

	public void InitialiseStats (int maxHealth, int attackDamage, float attackSpeed) {
		this.maxHealth = maxHealth;
		this.currentHealth = maxHealth;
		this.attackDamage = attackDamage;

		this.attackCooldown = 1f;
		this.nextAttackCounter = 0f;
	}

	public void HealUnit(int amount) {
		if (isDead)
			return;

		currentHealth += amount;
		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		}
	}

	public void DamageUnit(int amount) {
		currentHealth -= amount;
		if (currentHealth < 0) {
			currentHealth = 0;
			isDead = true;
		}
	}

	public int GetAttackDamage() {
		return attackDamage;
	}
}
