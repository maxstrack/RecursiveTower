using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
	public TextMeshProUGUI healthText;
    public int maxHealth = 10;
    private int currentHealth;
	public GameObject deathScreen;

	[Header("Damage Visuals")]
	public float invincibilityDuration = 1f;
	public SpriteRenderer playerRenderer; // Assign in inspector
	public Color flashColor = Color.red;
	public float flashDuration = 0.1f;

	private bool isInvincible = false;
	private Color originalColor;

	public TextMeshProUGUI scoreText;
	private int score = 0;


    void Start()
    {
	    currentHealth = maxHealth;
		UpdateUI();
		if (playerRenderer != null)
			originalColor = playerRenderer.color;
    }

	void UpdateUI()
	{
		if (healthText != null)
			healthText.text = "HP: " + currentHealth;
	}
	void Die()
	{
		Debug.Log("Player died!");
		if (deathScreen != null)
			deathScreen.SetActive(true);
		Time.timeScale = 0f; // Pause game
		GameManager.instance.TryUpdateHighScore(score);
	}

	public void RestartGame()
	{
		SceneManager.LoadScene("GameSene");
		Time.timeScale = 1f; // Unpause the game
	}

	public void ToMainMenu()
	{
		GameManager.instance.TryUpdateHighScore(score);
		SceneManager.LoadScene("MainMenu");
		Time.timeScale = 1f; // Unpause the game

		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			Destroy(enemy);
		}
	}

	public void TakeDamage(int amount)
	{
		if (isInvincible) return;

		currentHealth -= amount;
		UpdateUI();

		if (currentHealth <= 0)
		{
			Die();
		}
		else
		{
			StartCoroutine(DamageFlash());
			StartCoroutine(InvincibilityFrames());
		}
	}
	IEnumerator DamageFlash()
	{
		if (playerRenderer != null)
		{
			playerRenderer.color = flashColor;
			yield return new WaitForSeconds(flashDuration);
			playerRenderer.color = originalColor;
		}
	}
	IEnumerator InvincibilityFrames()
	{
		isInvincible = true;
		yield return new WaitForSeconds(invincibilityDuration);
		isInvincible = false;
	}

	public void AddScore(int amount)
	{
		score += amount;
		if (scoreText != null)
			scoreText.text = "Score: " + score;
	}

}

