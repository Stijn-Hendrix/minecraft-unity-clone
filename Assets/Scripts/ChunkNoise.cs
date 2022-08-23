using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkNoise
{
    ComputeBuffer noiseBuffer;

    ComputeShader noiseCompute;

    bool[] treesSample;

    const int tempWaterLevel = 232;


    public int[] Noise { get; private set; }

    int seed;

    public ChunkNoise(ComputeShader noiseCompute, int seed, Vector3 position) {
        this.noiseCompute = noiseCompute;
        this.seed = seed;
        this.treesSample = PoissonDiscSampling.Generate(6, 4, new Vector2Int(ChunkMetrics.chunkWidth + 2, ChunkMetrics.chunkWidth + 2), 30, seed + position.GetHashCode());

        CreateBuffers();

        GenerateNoise(position);

        Random.State state = Random.state;
        Random.InitState(seed + position.GetHashCode());

        PlaceTrees();

        Random.state = state;

        ReleaseBuffers();
	}


    void PlaceTrees() {
		for (int x = 0; x < ChunkMetrics.chunkWidth + 2; x++) {
            for (int z = 0; z < ChunkMetrics.chunkWidth + 2; z++) {
                int index0 = TreeIndexFromCoord(x, z);
                if (treesSample[index0]) {
                    for (int y = ChunkMetrics.chunkHeight - 1; y-- > tempWaterLevel;) {
                        int currentIndex = IndexFromCoord(x, y, z);
                        int aboveIndex = IndexFromCoord(x, y + 1, z);

                        if (Noise[currentIndex] == 3) {
                            if(Noise[aboveIndex] == 0) {
                                PlaceOakTree(x, y + 1, z);
                            }
						}
                    }
                }
            }
        }
	}

    void PlaceOakTree(int xCoord, int yCoord, int zCoord) {
        int rootLength = 4;

        // Leaves
        for (int x = -2; x < 2; x++) {
			for (int z = -2; z < 2; z++) {
				for (int y = -1; y < 1; y++) {
                    int leave = IndexFromCoord(xCoord + x, yCoord + rootLength + y, zCoord + z);
                    Noise[leave] = Random.value > 0.3f ? 6 : 0;
                }
            }
		}
        for (int x = -1; x < 1; x++) {
            for (int z = -1; z < 2; z++) {
                for (int y = 1; y < 2; y++) {
                    int leave = IndexFromCoord(xCoord + x, yCoord + rootLength + y, zCoord + z);
                    Noise[leave] = Random.value > 0.3f ? 6 : 0;
                }
            }
        }

        // Root
        for (int i = 0; i < rootLength; i++) {
            int root = IndexFromCoord(xCoord, yCoord + i, zCoord);
            Noise[root] = 5;
        }
    }

   

    int TreeIndexFromCoord(int x, int y) {
        return x + y * (ChunkMetrics.chunkWidth + 2);
    }


    int IndexFromCoord(int x, int y, int z) {
        return x + (ChunkMetrics.chunkWidth + 2) * (y + ChunkMetrics.chunkHeight * z);
    }

    void GenerateNoise(Vector3 position) {
        noiseCompute.SetBuffer(0, "_Noise", noiseBuffer);
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
