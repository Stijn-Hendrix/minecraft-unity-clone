using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1f;

    public Transform hand;
    public float handShakeSpeed = 1f;
    public float handShakeAmount = 1f;
    Vector3 originalHandpos;
    float handTime = 0f;

    CharacterController controller;

    Vector3 velocity;

	private void Awake() {
        controller = GetComponent<CharacterController>();

        originalHandpos = hand.transform.localPosition;
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

        if (move.magnitude > 0.1f) {

            handTime += Time.deltaTime * handShakeSpeed;
            Vector3 handpos = originalHandpos;
            handpos.y += Mathf.Sin(handTime) * handShakeAmount;
            handpos.x += Mathf.Cos(handTime) * handShakeAmount;
            hand.transform.localPosition = handpos;
            print("hi");
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
