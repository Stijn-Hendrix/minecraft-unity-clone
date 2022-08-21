using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1f;

    CharacterController controller;

    Vector3 velocity;

	private void Awake() {
        controller = GetComponent<CharacterController>();
	}


	void Update() {
        if (controller.isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1) {
            move /= move.magnitude;
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) {
            Jump();
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(move * speed * Time.deltaTime + velocity * Time.deltaTime);
    }

    void Jump() {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
