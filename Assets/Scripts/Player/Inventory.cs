using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	Dictionary<int, InventorySlot> blocks = new Dictionary<int, InventorySlot>();

	public InventoryUI inventoryUI;

	public int RemoveSelectedBlock() {
		InventorySlot slot = inventoryUI.CurrentlySelected.slot;
		if (slot == null) {
			return 0;
		}
		if (slot.count == 0) {
			return 0;
		}
		int selectedType = slot.blockType;
		slot.count -= 1;

		if (slot.count == 0) {
			inventoryUI.Remove(slot);
		}

		inventoryUI.CurrentlySelected.Refresh();

		return selectedType;
	}

	public void Add(int blockType) {
		if (blockType == 0) {
			return;
		}

		if (!blocks.ContainsKey(blockType)) {
			blocks[blockType] = new InventorySlot(blockType);
		}
		blocks[blockType].count += 1;

		inventoryUI.Add(blocks[blockType]);
		inventoryUI.Refresh();
	}
}

public class InventorySlot {
	public int blockType;
	public int count;

	public InventorySlot(int blockType, int count = 0) {
		this.blockType = blockType;
		this.count = count;
	}
}