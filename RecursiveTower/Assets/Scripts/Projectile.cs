using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 direction;
    public float speed;
    public SpellNode spellNodeUI;
    public GameObject playerReference;

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && spellNodeUI != null && playerReference != null)
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
				/*
				// For Spell Building
                SpellContext ctx = new SpellContext(playerReference.GetComponent<PlayerHealth>(), enemy);
                var runtimeSpell = spellNodeUI.ToRuntimeNode();
                runtimeSpell?.Evaluate(ctx);
				*/
				enemy.TakeDamage(1);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}

