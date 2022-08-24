using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public List<HotbarItem> hotbarItems;

	HashSet<InventorySlot> slots = new HashSet<InventorySlot>();

	public HotbarItem CurrentlySelected => hotbarItems[selectedHotbarItem];

	int selectedHotbarItem = 0;

	private void Start() {
		SelectHotbarItem(0);
	}

	public void Add(InventorySlot slot) {
		if (slots.Contains(slot)) {
			return;
		}
		slots.Add(slot);

		for (int i = 0; i < hotbarItems.Count; i++) {
			if (hotbarItems[i].slot == null) {
				hotbarItems[i].slot = slot;
				return;
			}
		}
	}

	public void Remove(InventorySlot slot) {
		if (slots.Contains(slot)) {
			slots.Remove(slot);
		}
		for (int i = 0; i < hotbarItems.Count; i++) {
			if (hotbarItems[i].slot == slot) {
				hotbarItems[i].slot = null;
				hotbarItems[i].Refresh();
				return;
			}
		}
	}

    public void Refresh() {
		for (int i = 0; i < hotbarItems.Count; i++) {
			if (hotbarItems[i].slot != null) {
				hotbarItems[i].Refresh();
			}
		}
	}

	void SelectHotbarItem(int index) {
		for (int i = 0; i < hotbarItems.Count; i++) {
			hotbarItems[i].Highlight(i == index);
		}
	}

	private void Update() {
		float scrollDelta = Input.mouseScrollDelta.y;

		if (scrollDelta == 0) {
			return;
		}

		int scroll = Mathf.RoundToInt(scrollDelta);

		selectedHotbarItem -= scroll;
		selectedHotbarItem = Mathf.Clamp(selectedHotbarItem, 0, hotbarItems.Count - 1);
		SelectHotbarItem(selectedHotbarItem);
	}
}
