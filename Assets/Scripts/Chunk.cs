using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("-- Display --")]
    public MeshFilter meshFilter;

    [Header("-- Compute --")]
    public ComputeShader triangulateShader;
    public ComputeShader noiseShader;

    [Header("-- Terrain --")]
    public int seed;
    public BlockTypeAtlas blockAtlas;

    ChunkMesh chunkMesh;
    ChunkNoise chunkNoise;

	[ContextMenu("Create")]
    public void Create() {
      

        chunkNoise = new ChunkNoise(noiseShader, seed, transform.localPosition);

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        chunkMesh = new ChunkMesh(blockAtlas, triangulateShader);

        meshFilter.mesh = chunkMesh.Create(chunkNoise.Noise);

        stopwatch.Stop();
        //print($"Chunk creation - Time elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
    }
	
	int IndexFromCoord(int x, int y, int z) {
        return x + ChunkMetrics.chunkWidth * (y + ChunkMetrics.chunkWidth * z);
    }
}
