using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
	public static int chunksCreated;
	public static float totalCreationTime;

	public Text chunkCreationText;

	private void Awake() {
		chunksCreated = 0;
		totalCreationTime = 0;
	}

	private void LateUpdate() {
		chunkCreationText.text = $"avg {totalCreationTime / chunksCreated} ms"; 
	}
}
