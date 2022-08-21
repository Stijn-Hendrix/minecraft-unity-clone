using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkNoise
{
    ComputeBuffer noiseBuffer;

    ComputeShader noiseCompute;

    public int[] Noise { get; private set; }

    int seed;

    public ChunkNoise(ComputeShader noiseCompute, int seed, Vector3 position) {
        this.noiseCompute = noiseCompute;
        this.seed = seed;

        CreateBuffers();

        GenerateNoise(position);

        ReleaseBuffers();
	}

    void GenerateNoise(Vector3 position) {
        noiseCompute.SetBuffer(0, "_Noise", noiseBuffer);
        //noiseCompute.SetInt("_ChunkSize", ChunkMetrics.chunkSize);
        noiseCompute.SetInt("_ChunkSizeWidth", ChunkMetrics.chunkWidth);
        noiseCompute.SetInt("_ChunkSizeHeight", ChunkMetrics.chunkHeight);
        noiseCompute.SetInt("_NoiseSeed", seed);
        noiseCompute.SetVector("_PositionOffset", position);

        noiseCompute.Dispatch(0, ChunkMetrics.dispatchThreadsWidth + 1, ChunkMetrics.dispatchThreadsHeight, ChunkMetrics.dispatchThreadsWidth + 1);

        Noise = new int[ChunkMetrics.noisePerChunk];
        noiseBuffer.GetData(Noise);
    }

    void CreateBuffers() {
        noiseBuffer = new ComputeBuffer(ChunkMetrics.noisePerChunk, sizeof(int));
    }

    void ReleaseBuffers() {
        noiseBuffer.Release();
    }
}
