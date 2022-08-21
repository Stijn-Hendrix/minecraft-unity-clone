using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
	public Chunk chunkPrefab;

	public int renderDistance;

	public Transform player;

	Dictionary<Vector2Int, Chunk> chunksDict = new Dictionary<Vector2Int, Chunk>();

	Queue<Chunk> buildQueue = new Queue<Chunk>();
	float buildTime = 1f;

	Vector2Int currentPlayerChunk = Vector2Int.one;
	Vector2Int previousPlayerChunk = Vector2Int.zero;

	private void Start() {
		previousPlayerChunk = Vector2Int.one * 100;
		currentPlayerChunk = Vector2Int.one * -100;

		UpdateChunks();

		// Instantly build everything in the build queue
		while (buildQueue.Count > 0) {
			var chunk = buildQueue.Dequeue();
			chunk.Create();
		}
	}

	private void Update() {
		UpdateChunks();

		// Update the build queue
		buildTime -= Time.deltaTime * 150f;
		if (buildTime <= 0) {
			if (buildQueue.Count > 0) {
				var chunk = buildQueue.Dequeue();
				chunk.Create();
			}
			buildTime = 1f;
		}
	}

	private void UpdateChunks() {
		Vector3 playerPosition = player.transform.position;

		currentPlayerChunk = ChunkFromWorldPosition(playerPosition.x, playerPosition.z);

		if (currentPlayerChunk != previousPlayerChunk) {

			foreach (var keypair in chunksDict) {
				if (!InRenderDistance(keypair.Value.Position, currentPlayerChunk, renderDistance)) {
					keypair.Value.Visible = false;
				}
			}

			for (int x = -renderDistance; x <= renderDistance; x++) {
				for (int z = -renderDistance; z <= renderDistance; z++) {
					Vector2Int chunkCoord = new Vector2Int(currentPlayerChunk.x + x, currentPlayerChunk.y + z);

					if (chunksDict.ContainsKey(chunkCoord)) {
						chunksDict[chunkCoord].Visible = true;
					}
					else {
						var chunk = CreateChunk(chunkCoord.x, chunkCoord.y);
						chunksDict.Add(chunkCoord, chunk);
						chunk.Visible = true;
					}
				}
			}

			previousPlayerChunk = currentPlayerChunk;
		}
	}

	public Chunk GetChunk(int x, int z) {
		Vector2Int key = new Vector2Int(x, z);
		if (chunksDict.ContainsKey(key)) {
			return chunksDict[key];
		}
		return null;
	}

	Chunk CreateChunk(int x, int z) {

		Chunk chunk = Instantiate(chunkPrefab, transform);

		chunk.transform.localPosition = new Vector3(x * ChunkMetrics.chunkWidth, 0, z * ChunkMetrics.chunkWidth);
		chunk.Position = new Vector2Int(x, z);
		chunk.name = $"{x}:{z}";
		//chunk.Create();

		// Queue the build process to avoid stuttering caused by creating to many chunks at the exact
		// same time
		buildQueue.Enqueue(chunk);

		return chunk;
	}

	public static Vector2Int ChunkFromWorldPosition(float x, float z) {
		Vector2Int v = new Vector2Int(
			Mathf.FloorToInt((x - 1) / (ChunkMetrics.chunkWidth)),
			Mathf.FloorToInt((z - 1) / (ChunkMetrics.chunkWidth))
		);
		return v;
	}

	public static Vector3Int LocalPositionFromWorldPosition(Chunk chunk, Vector3 globalPosition) {
		int x = Mathf.FloorToInt(globalPosition.x) - (ChunkMetrics.chunkWidth * chunk.Position.x);
		int y = Mathf.FloorToInt(globalPosition.y);
		int z = Mathf.FloorToInt(globalPosition.z) - (ChunkMetrics.chunkWidth * chunk.Position.y);

		return new Vector3Int(x, y, z);
	}

	bool InRenderDistance(Vector2Int a, Vector2Int b, int chunkRenderingDistance) {
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) <= chunkRenderingDistance;
	}
}
