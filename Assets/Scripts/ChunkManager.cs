using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
	public Chunk chunkPrefab;

	public int chunks;

	private void Start() {

		int c = Mathf.RoundToInt(chunks / 2);

		for (int x = -c; x < c; x++) {
			for (int z = -c; z < c; z++) {
				CreateChunk(x, z);
			}
		}

	}

	void CreateChunk(int x, int z) {

		Chunk chunk = Instantiate(chunkPrefab, transform);

		chunk.transform.localPosition = new Vector3(x * ChunkMetrics.chunkWidth, 0, z * ChunkMetrics.chunkWidth);

		chunk.Create();
	}
}
