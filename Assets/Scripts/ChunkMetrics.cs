using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkMetrics
{
    public const int computeThreads = 8;

    public const int dispatchThreadsWidth = chunkWidth / computeThreads;
    public const int dispatchThreadsHeight = chunkHeight / computeThreads;

    // must be divible by 8
    public const int chunkWidth = 16;
    public const int chunkHeight = 128;

    public const int blocksPerChunk = chunkWidth * chunkWidth * chunkHeight;
    public const int noisePerChunk = (chunkWidth + 2) * (chunkWidth + 2) * chunkHeight;
}
