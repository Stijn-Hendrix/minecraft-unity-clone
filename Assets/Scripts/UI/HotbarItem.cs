using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarItem : MonoBehaviour
{
    public InventorySlot slot;

    public BlockTypeAtlas atlas;

    public Image borderImage;
    public Image blockImage;
    public Text countText;

    Color originalBorderColor;

	private void Awake() {
        originalBorderColor = borderImage.color;
    }

	public void Highlight(bool on) {
        borderImage.color = on ? Color.white : originalBorderColor;
	}

    public void Refresh() {
        if (slot == null) {
            blockImage.enabled = false;
            countText.text = "";
            return;
		}
        if (slot.count == 0) {
            blockImage.enabled = false;
            countText.text = "";
            return;
        }
        blockImage.enabled = true;
        blockImage.sprite = atlas.BlockTypes[slot.blockType].inventorySprite;
        countText.text = slot.count.ToString();
    }
}
