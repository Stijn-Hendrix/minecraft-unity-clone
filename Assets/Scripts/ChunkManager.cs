using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
	public Chunk chunkPrefab;

	[Range(2, 20)]
	public int renderDistance;

	public Transform player;

	Dictionary<Vector2Int, Chunk> chunksDict = new Dictionary<Vector2Int, Chunk>();

	//Queue<Chunk> buildQueue = new Queue<Chunk>();
	ChunkPriorityQueue buildQueue = new ChunkPriorityQueue();

	Queue<Chunk> chunkPool = new Queue<Chunk>();

	Vector2Int currentPlayerChunk = Vector2Int.one;
	Vector2Int previousPlayerChunk = Vector2Int.zero;

	private void Start() {
		previousPlayerChunk = Vector2Int.one * 100;
		currentPlayerChunk = Vector2Int.one * -100;

		UpdateChunks();

		var stopwatch = new System.Diagnostics.Stopwatch();
		stopwatch.Start();
		// Build most of the build queue

		int count = buildQueue.Count;
		for (int i = 0; i < count; i++) {
			var chunk = buildQueue.Dequeue();
			if (InRenderDistance(currentPlayerChunk, chunk.Position, Mathf.Max(renderDistance / 2, 2))) {
				chunk.Create();
				chunk.Visible = true;
			}
			else {
				buildQueue.Enqueue(chunk, ManhattanDistance(chunk.Position, currentPlayerChunk));
			}
		}

		
		stopwatch.Stop();
		print($"Initial creation - Time elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
	}

	private void Update() {
		UpdateChunks();

		for (int i = 0; i < 2; i++) {
			if (buildQueue.Count > 0) {
				var chunk = buildQueue.Dequeue();
				if (chunk) {
					chunk.Create();
					chunk.Visible = true;
				}
			}
		}
	}

	private void UpdateChunks() {

		Vector3 playerPosition = player.transform.position;

		currentPlayerChunk = ChunkFromWorldPosition(playerPosition.x, playerPosition.z);

		if (currentPlayerChunk != previousPlayerChunk) {

			// Remove chunks out of render distance
			List<Chunk> chunkRemove = ListPool<Chunk>.Get();
			foreach (var keypair in chunksDict) {
				Chunk chunk = keypair.Value;
				if (!InRenderDistance(chunk.Position, currentPlayerChunk, renderDistance)) {
					if (!chunk.HasBeenEdited) {
						chunkRemove.Add(chunk);
						chunkPool.Enqueue(chunk);
					}
					chunk.Visible = false;
				}	
			}
			for (int i = 0; i < chunkRemove.Count; i++) {
				chunksDict.Remove(chunkRemove[i].Position);
			}
			ListPool<Chunk>.Add(chunkRemove);

			// Create chunks in render distance
			for (int x = -renderDistance; x <= renderDistance; x++) {
				for (int z = -renderDistance; z <= renderDistance; z++) {
					Vector2Int chunkCoord = new Vector2Int(currentPlayerChunk.x + x, currentPlayerChunk.y + z);

					if (chunksDict.ContainsKey(chunkCoord)) {
						chunksDict[chunkCoord].Visible = true;
					}
					else {
						var chunk = CreateChunk(chunkCoord.x, chunkCoord.y);
						chunksDict.Add(chunkCoord, chunk);
						chunk.Visible = false;
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


		Chunk chunk = chunkPool.Count > 0 ? chunkPool.Dequeue() : Instantiate(chunkPrefab, transform);

		chunk.transform.localPosition = new Vector3(x * ChunkMetrics.chunkWidth, 0, z * ChunkMetrics.chunkWidth);
		chunk.Position = new Vector2Int(x, z);
		chunk.name = $"{x}:{z}";
		//chunk.Create();

		// Queue the build process to avoid stuttering caused by creating to many chunks at the exact
		// same time
		buildQueue.Enqueue(chunk, ManhattanDistance(chunk.Position, currentPlayerChunk));

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
		return Mathf.Abs(a.x - b.x) <= chunkRenderingDistance && Mathf.Abs(a.y - b.y) <= chunkRenderingDistance;
	}

	public static int ManhattanDistance(Vector2Int a, Vector2Int b) {
		checked {
			return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
		}
	}

}
