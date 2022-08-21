using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
	public int fps = 120;

	private void Awake() {
		if (fps <= 0) {
			return;
		}
		Application.targetFrameRate = fps;
	}
}
