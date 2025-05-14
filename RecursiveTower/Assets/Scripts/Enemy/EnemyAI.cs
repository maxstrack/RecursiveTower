using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Chase Settings")]
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float trackingDistance = 6f;
    public float stopDistance = 1.5f;

    [Header("Orbit Settings")]
    public float minOrbitSpeed = 60f;
    public float maxOrbitSpeed = 180f;
    public float directionChangeInterval = 2f; // seconds

    private float speed;
    private float orbitSpeed;
    private int orbitDirection; // 1 or -1

    private Transform player;
    private float timeSinceDirectionFlip = 0f;

	public GameObject projectilePrefab;
	public float fireInterval = 2f;
	private float timeSinceLastShot = 0f;
	[Header("Firing Settings")]
	public float firingDistance = 4f; // Enemy only fires within this distance

	[Header("Wandering")]
	public float wanderSpeed = 1.5f;
	public float wanderInterval = 2f;
	public float wanderRadius = 3f;

	private Vector2 wanderTarget;
	private float timeSinceWander;

	private Rigidbody2D rb;

    void Start()
    {
		rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Randomize speed and orbit speed
        speed = Random.Range(minSpeed, maxSpeed);
        orbitSpeed = Random.Range(minOrbitSpeed, maxOrbitSpeed);
        orbitDirection = Random.value < 0.5f ? 1 : -1;
    }

	void Update()
	{
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);

		if (distanceToPlayer <= trackingDistance)
		{
			if (distanceToPlayer > stopDistance)
				ChasePlayer();
			else
				OrbitAroundPlayer();
		}
		else
		{
			Wander();
		}
		HandleFiring();
	}

	void HandleFiring()
	{
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);
		if (distanceToPlayer <= firingDistance)
		{
			timeSinceLastShot += Time.deltaTime;
			if (timeSinceLastShot >= fireInterval)
			{
				Vector2 fireDirection = (player.position - transform.position).normalized;
				GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
				EnemyProjectile script = proj.GetComponent<EnemyProjectile>();
				script.direction = fireDirection;
				timeSinceLastShot = 0f;
			}
		}
	}

	void OrbitAroundPlayer()
	{
		float angle = orbitSpeed * orbitDirection * Time.deltaTime;
		transform.RotateAround(player.position, Vector3.forward, angle);

		// Prevent the sprite from rotating
		transform.rotation = Quaternion.identity;

		timeSinceDirectionFlip += Time.deltaTime;
		if (timeSinceDirectionFlip >= directionChangeInterval)
		{
			if (Random.value < 0.3f)
			{
				orbitDirection *= -1;
				orbitSpeed = Random.Range(minOrbitSpeed, maxOrbitSpeed);
			}
			timeSinceDirectionFlip = 0f;
		}
	}

	void ChasePlayer()
	{
		Vector2 direction = (player.position - transform.position).normalized;
		rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
	}

	void Wander()
	{
		timeSinceWander += Time.deltaTime;

		if (timeSinceWander >= wanderInterval || wanderTarget == Vector2.zero)
		{
			// Pick a new wander target
			Vector2 randomOffset = Random.insideUnitCircle.normalized * wanderRadius;
			wanderTarget = (Vector2)transform.position + randomOffset;
			timeSinceWander = 0f;
		}

		// Move toward the wander target
		Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
		rb.MovePosition(rb.position + direction * wanderSpeed * Time.deltaTime);

		// Optionally: reset target if close enough
		if (Vector2.Distance(transform.position, wanderTarget) < 0.5f)
		{
			wanderTarget = Vector2.zero;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// Ensure it's the PLAYER and a trigger collider
		if (other.CompareTag("Player") && other.isTrigger)
		{
			PlayerHealth player = other.GetComponent<PlayerHealth>();
			if (player != null)
			{
				player.TakeDamage(1);
			}

		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			orbitDirection *= -1;
		}
	}
}
