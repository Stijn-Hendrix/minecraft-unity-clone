using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkMetrics
{
    public const int computeThreads = 8;

    public const int dispatchThreads = chunkSize / computeThreads;

    // must be divible by 8
    public const int chunkSize = 32;

    public const int blocksPerChunk = chunkSize * chunkSize * chunkSize;
}
