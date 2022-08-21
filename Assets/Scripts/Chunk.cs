using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
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
        if (showCreationTime) {
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
        }
    }

    void FinishCreationStopWatch() {
        if (showCreationTime) {
            stopwatch.Stop();
            print($"Chunk creation - Time elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
        }
    }

    void FinishRefreshStopWatch() {
        if (showCreationTime) {
            stopwatch.Stop();
            print($"Chunk refresh - Time elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
        }
    }
}
