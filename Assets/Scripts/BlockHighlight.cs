using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHighlight : MonoBehaviour
{
    public Material breakMat;

    public int textures;

    const float textureSizeInPixels = 16f;

	float scrollOffset;

	public GameObject breakBlock;
	public GameObject highlightBlock;

	public bool BreakVisible {
		get {
			return breakBlock.activeSelf;
		}
		set {
			breakBlock.SetActive(value);

			
		}
	}


	public bool Visible {
		get {
			return highlightBlock.activeSelf;
		}
		set {
			highlightBlock.SetActive(value);
		}
	}

	private void Awake() {
		scrollOffset = textures / textureSizeInPixels;
	}

	private void OnEnable() {
		scrollOffset = textures / textureSizeInPixels;
	}

	private void Start() {
		SetProgress(0);
	}

	public void SetProgress(int index) {
		index = Mathf.Clamp(index, 0, textures);
		breakMat.mainTextureOffset = new Vector2(index * scrollOffset, 1);
	}

	public void SetProgress(float percent) {
		float offsetPerc = scrollOffset * 100f;

		int index = Mathf.FloorToInt(percent / offsetPerc);
		SetProgress(index);
	}
}
