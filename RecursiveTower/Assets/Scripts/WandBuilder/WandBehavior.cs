using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.3f;

    private float fireTimer = 0f;

    private WandMenuController wandMenu;

    void Start()
    {
        wandMenu = FindObjectOfType<WandMenuController>(); // or set via inspector
    }

    void Update()
    {
        Vector2 direction = GetArrowKeyDirection();
        fireTimer += Time.deltaTime;

        if (direction != Vector2.zero && fireTimer >= fireRate)
        {
            FireProjectile(direction);
            fireTimer = 0f;
        }
    }

    void FireProjectile(Vector2 direction)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projScript = proj.GetComponent<Projectile>();
        projScript.direction = direction.normalized;
        projScript.speed = projectileSpeed;

        // Connect the spell
        SpellNode rootSpell = wandMenu.GetRootSpellNode();
        projScript.spellNodeUI = rootSpell;
        projScript.playerReference = this.gameObject; // if player logic is on this GameObject
    }

    Vector2 GetArrowKeyDirection()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))  x -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) x += 1f;
        if (Input.GetKey(KeyCode.UpArrow))    y += 1f;
        if (Input.GetKey(KeyCode.DownArrow))  y -= 1f;

        return new Vector2(x, y).normalized;
    }
}
