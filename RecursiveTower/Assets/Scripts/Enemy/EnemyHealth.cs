using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
	private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

	public void TakeDamage(int amount)
	{
		if (isDead) return;

		currentHealth -= amount;

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		if (isDead) return;
		isDead = true;

		PlayerHealth player = FindObjectOfType<PlayerHealth>();
		if (player != null)
		{
			player.AddScore(1);
		}

		Destroy(gameObject);
	}

}

