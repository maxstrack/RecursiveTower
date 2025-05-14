using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 10f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Player"))
		{
			Debug.Log("Projectile hit something: " + other.name);

			if (other.CompareTag("Enemy"))
			{
				Debug.Log("Projectile hit " + other.name);
				EnemyHealth enemy = other.GetComponent<EnemyHealth>();
				if (enemy != null)
				{
					enemy.TakeDamage(1);
				}

			}
			Destroy(gameObject);
		}
	}
}

