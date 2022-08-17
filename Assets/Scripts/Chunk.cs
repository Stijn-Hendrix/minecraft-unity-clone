using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshFilter meshFilter;
    public BlockTypeAtlas blockAtlas;

    public ComputeShader triangulateShader;
    public ComputeShader noiseShader;

    ChunkMesh chunkMesh;
    ChunkNoise chunkNoise;

    public int seed;

    void Start()
    {
        Create();
    }

	[ContextMenu("Create")]
    public void Create() {
      

        chunkNoise = new ChunkNoise(noiseShader, seed);

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        chunkMesh = new ChunkMesh(blockAtlas, triangulateShader);

        meshFilter.mesh = chunkMesh.Create(chunkNoise.Noise);

        stopwatch.Stop();
        print($"Chunk creation - Time elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
    }

	private void OnValidate() {
        Create();
	}

	int IndexFromCoord(int x, int y, int z) {
        return x + ChunkMetrics.chunkWidth * (y + ChunkMetrics.chunkWidth * z);
    }
}
