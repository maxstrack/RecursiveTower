using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb2d;
    private Vector2 moveInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		/*
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        rb2d.velocity = moveInput * moveSpeed;
		*/

		moveInput = Vector2.zero;

		if (Input.GetKey(KeyCode.W))
			moveInput.y += 1;
		if (Input.GetKey(KeyCode.S))
			moveInput.y -= 1;
		if (Input.GetKey(KeyCode.D))
			moveInput.x += 1;
		if (Input.GetKey(KeyCode.A))
			moveInput.x -= 1;

		moveInput.Normalize();

		rb2d.velocity = moveInput * moveSpeed;

    }
}
