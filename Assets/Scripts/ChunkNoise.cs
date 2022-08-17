using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkNoise
{
    ComputeBuffer noiseBuffer;

    ComputeShader noiseCompute;

    public int[] Noise { get; private set; }

    int seed;

    public ChunkNoise(ComputeShader noiseCompute, int seed) {
        this.noiseCompute = noiseCompute;
        this.seed = seed;

        CreateBuffers();

        GenerateNoise();

        ReleaseBuffers();
	}

    void GenerateNoise() {
        noiseCompute.SetBuffer(0, "_Noise", noiseBuffer);
        //noiseCompute.SetInt("_ChunkSize", ChunkMetrics.chunkSize);
        noiseCompute.SetInt("_ChunkSizeWidth", ChunkMetrics.chunkWidth);
        noiseCompute.SetInt("_ChunkSizeHeight", ChunkMetrics.chunkHeight);
        noiseCompute.SetInt("_NoiseSeed", seed);

        noiseCompute.Dispatch(0, ChunkMetrics.dispatchThreadsWidth, ChunkMetrics.dispatchThreadsHeight, ChunkMetrics.dispatchThreadsWidth);

        Noise = new int[ChunkMetrics.blocksPerChunk];
        noiseBuffer.GetData(Noise);
    }

    void CreateBuffers() {
        noiseBuffer = new ComputeBuffer(ChunkMetrics.blocksPerChunk, sizeof(int));
    }

    void ReleaseBuffers() {
        noiseBuffer.Release();
    }
}
