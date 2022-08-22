using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1f;

    public Animator handHandimator;

    CharacterController controller;

    bool creativeMode = false;

    Vector3 velocity;

	private void Awake() {
        controller = GetComponent<CharacterController>();
	}

    void HandleInput() {
        if (Input.GetKeyDown(KeyCode.C)) {
            creativeMode = !creativeMode;

            if (!creativeMode) {
                velocity.y = -2f;
            }
        }
	}

	void Update() {
        HandleInput();

        if (!creativeMode && controller.isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
        else if(creativeMode) {
            velocity.y = 0f;
		}

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1) {
            move /= move.magnitude;
        }

        handHandimator.SetFloat("move", move.magnitude);

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) {
            if (!creativeMode) {
                Jump();
            }
			else {
			}
        }
        if (creativeMode) {
            if (Input.GetKey(KeyCode.Space)) {
                velocity.y = jumpHeight * 3;
		    }
            if (Input.GetKey(KeyCode.LeftShift)) {
                velocity.y = -jumpHeight * 3;
            }
        }

        if (!creativeMode) {
            velocity.y += gravity * Time.deltaTime;
		}

        controller.Move(move * ((creativeMode && !controller.isGrounded) ? speed * 3 : speed) * Time.deltaTime + velocity * Time.deltaTime);
    }

    void Jump() {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
