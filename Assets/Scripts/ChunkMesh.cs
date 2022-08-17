using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMesh
{
    ComputeBuffer trianglesBuffer;
    ComputeBuffer trianglesCountBuffer;
    ComputeBuffer blockTypeAtlasBuffer;
    ComputeBuffer blocksBuffer;

    BlockTypeAtlas blockAtlas;

    ComputeShader triangulateShader;


    public Mesh Mesh { get; private set; }

    private int TriangleCount {
        get {
            int[] triCount = { 0 };
            ComputeBuffer.CopyCount(trianglesBuffer, trianglesCountBuffer, 0);
            trianglesCountBuffer.GetData(triCount);
            return triCount[0];
        }
	}

    public ChunkMesh(BlockTypeAtlas blockAtlas, ComputeShader triangulateShader) {
        this.blockAtlas = blockAtlas;
        this.triangulateShader = triangulateShader;
    }

    public Mesh Create(int[] blockTypes) {
        CreateBuffers();

        Triangulate(blockTypes);

        ReleaseBuffers();

        return Mesh;
    }

    void Triangulate(int[] blockTypes) {
        // Reset triangles buffer data
        trianglesBuffer.SetCounterValue(0);

        // Set buffers
        triangulateShader.SetBuffer(0, "_Triangles", trianglesBuffer);
        triangulateShader.SetBuffer(0, "_Blocks", blocksBuffer);
        triangulateShader.SetBuffer(0, "_BlocksAtlas", blockTypeAtlasBuffer);
        triangulateShader.SetInt("_ChunkSizeWidth", ChunkMetrics.chunkWidth);
        triangulateShader.SetInt("_ChunkSizeHeight", ChunkMetrics.chunkHeight);

        // Set block data
        blockTypeAtlasBuffer.SetData(blockAtlas.BlockTypeUvs);
        blocksBuffer.SetData(blockTypes);
                
        // Dispatch
        triangulateShader.Dispatch(0, ChunkMetrics.dispatchThreadsWidth, ChunkMetrics.dispatchThreadsHeight, ChunkMetrics.dispatchThreadsWidth);

        // Retreive triangle data
        Triangle[] triangles = new Triangle[TriangleCount];
        trianglesBuffer.GetData(triangles);

        CreateMesh(triangles);
    }

    void CreateMesh(Triangle[] triangles) {
        Vector3[] verts = new Vector3[triangles.Length * 3];
        Vector2[] uvs = new Vector2[triangles.Length * 3];
        int[] tris = new int[triangles.Length * 3];

        for (int i = 0; i < triangles.Length; i++) {
            int startIndex = i * 3;

            verts[startIndex] = triangles[i].v_a;
            verts[startIndex + 1] = triangles[i].v_b;
            verts[startIndex + 2] = triangles[i].v_c;

            uvs[startIndex] = triangles[i].uv_a;
            uvs[startIndex + 1] = triangles[i].uv_b;
            uvs[startIndex + 2] = triangles[i].uv_c;

            tris[startIndex] = startIndex;
            tris[startIndex + 1] = startIndex + 1;
            tris[startIndex + 2] = startIndex + 2;
        }

        if (Mesh == null) {
            Mesh = new Mesh();
		}
        Mesh.Clear();
        Mesh.vertices = verts;
        Mesh.triangles = tris;
        Mesh.uv = uvs;
        Mesh.RecalculateNormals();
    }

    void CreateBuffers() {
        trianglesBuffer = new ComputeBuffer(ChunkMetrics.blocksPerChunk * 12, Triangle.SizeOf, ComputeBufferType.Append);
        trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        blocksBuffer = new ComputeBuffer(ChunkMetrics.blocksPerChunk, sizeof(int));
        blockTypeAtlasBuffer = new ComputeBuffer(blockAtlas.BlockUvs.Length, BlockTypeUvs.SizeOf);
    }

    void ReleaseBuffers() {
        trianglesBuffer.Release();
        trianglesCountBuffer.Release();
        blocksBuffer.Release();
        blockTypeAtlasBuffer.Release();
    }

    struct Triangle {
        public Vector3 v_a;
        public Vector3 v_b;
        public Vector3 v_c;

        public Vector2 uv_a;
        public Vector2 uv_b;
        public Vector2 uv_c;

        public static int SizeOf => (sizeof(float) * 3 * 3) + (sizeof(float) * 2 * 3);
    }
}
