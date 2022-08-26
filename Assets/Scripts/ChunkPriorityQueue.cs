using System.Collections.Generic;

public class ChunkPriorityQueue {

	List<Queue<Chunk>> list = new List<Queue<Chunk>>();


	int count = 0;
	int minimum = int.MaxValue;

	public int Count {
		get {
			return count;
		}
	}

	public void Enqueue (Chunk chunk, int priority) {
		count += 1;
		if (priority < minimum) {
			minimum = priority;
		}
		while (priority >= list.Count) {
			list.Add(null);
		}
		if (list[priority] == null) {
			list[priority] = new Queue<Chunk>();
		}
		list[priority].Enqueue(chunk);
	}

	public Chunk Dequeue () {
		count -= 1;
		for (; minimum < list.Count; minimum++) {
			Queue<Chunk> cell = list[minimum];
			if (cell != null && cell.Count > 0) {
				return cell.Dequeue();
			}
		}
		return null;
	}

	public void Clear () {
		list.Clear();
		count = 0;
		minimum = int.MaxValue;
	}
}