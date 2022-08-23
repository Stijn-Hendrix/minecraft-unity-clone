using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector2Int Position { get; set; }

    public bool Visible {
        get {
            return gameObject.activeSelf;
        }
        set {
            if (value != Visible) {
                gameObject.SetActive(value);
            }
        }
    }

    [Header("-- Display --")]
    public bool showCreationTime = false;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    [Header("-- Compute --")]
    public ComputeShader triangulateShader;
    public ComputeShader noiseShader;

    [Header("-- Terrain --")]
    public int seed;
    public BlockTypeAtlas blockAtlas;

    ChunkMesh chunkMesh;
    ChunkNoise chunkNoise;

    public int[] Blocks => chunkNoise.Noise;

    System.Diagnostics.Stopwatch stopwatch;

    [ContextMenu("Create")]
    public void Create() {
        StartStopWatch();

        chunkNoise = new ChunkNoise(noiseShader, seed, transform.localPosition);
        chunkMesh = new ChunkMesh(blockAtlas, triangulateShader);

        meshFilter.sharedMesh = 
            meshCollider.sharedMesh = chunkMesh.Create(Blocks);

        FinishCreationStopWatch();

        enabled = false;
    }

    public void Cleanup() {
        if (meshCollider.sharedMesh != null) {
            meshCollider.sharedMesh.Clear();
            meshFilter.sharedMesh.Clear();
		}

        chunkNoise = null;
        chunkMesh = null;
    }

    public void Refresh() {
        enabled = true;
	}

	private void LateUpdate() {
        StartStopWatch();

        meshFilter.sharedMesh =
            meshCollider.sharedMesh = chunkMesh.Create(Blocks);

        FinishRefreshStopWatch();
        enabled = false;
    }

	void StartStopWatch() {
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
    }

    void FinishCreationStopWatch() {
        stopwatch.Stop();
        if (showCreationTime) {
            print($"Chunk creation - Time elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        DebugUI.totalCreationTime += (float)stopwatch.Elapsed.TotalMilliseconds;
        DebugUI.chunksCreated += 1;

    }

    void FinishRefreshStopWatch() {
        stopwatch.Stop();
        if (showCreationTime) {
            print($"Chunk refresh - Time elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
        }
    }


}
