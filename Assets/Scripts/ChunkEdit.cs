using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkEdit : MonoBehaviour
{
	public Camera cam;
    public ChunkManager chunkManager;

	private void Update() {

		if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RemoveBlock();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            AddBlock();
        }
    }

    void RemoveBlock() {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {

            var hitChunk = hit.transform.GetComponent<Chunk>();

            Vector3 offsetPosition = hit.point + cam.transform.forward * 0.1f;

            Vector3Int localPosition = LocalPositionFromGlobal(hitChunk, offsetPosition);
            EditChunk(hitChunk, localPosition, 0);
        }
    }

    void AddBlock() {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {

            var hitChunk = hit.transform.GetComponent<Chunk>();

            Vector3 offsetPosition = hit.point - cam.transform.forward * 0.1f;

            Vector3Int localPosition = LocalPositionFromGlobal(hitChunk, offsetPosition);
            EditChunk(hitChunk, localPosition, 1);
        }
    }

    void EditChunk(Chunk chunk, Vector3Int localPosition, int newBlockType = 0) {
        if (chunk.Blocks[IndexFromCoord(localPosition.x, localPosition.y, localPosition.z)] == newBlockType) {
            return;
		}
        chunk.Blocks[IndexFromCoord(localPosition.x, localPosition.y, localPosition.z)] = newBlockType;
        chunk.Refresh();

        Vector2Int chunkPosition = chunk.Position;

        if (localPosition.x == 1) {
            Chunk neighbour = chunkManager.GetChunk(chunkPosition.x - 1, chunkPosition.y);

            if (neighbour) {
                // Update the edge at x = chunkWidth + 1
                neighbour.Blocks[IndexFromCoord(ChunkMetrics.chunkWidth + 1, localPosition.y, localPosition.z)] = newBlockType;
                neighbour.Refresh();
            }
        }


        if (localPosition.z == 1) {
            Chunk neighbour = chunkManager.GetChunk(chunkPosition.x, chunkPosition.y - 1);

            if (neighbour) {
                // Update the edge at z = chunkWidth + 1
                neighbour.Blocks[IndexFromCoord(localPosition.x, localPosition.y, ChunkMetrics.chunkWidth + 1)] = newBlockType;
                neighbour.Refresh();
            }
        }

        if (localPosition.x == ChunkMetrics.chunkWidth) {
            Chunk neighbour = chunkManager.GetChunk(chunkPosition.x + 1, chunkPosition.y);

            if (neighbour) {
                // Update the edge at x = 0
                neighbour.Blocks[IndexFromCoord(0, localPosition.y, localPosition.z)] = newBlockType;
                neighbour.Refresh();
            }

        }

        if (localPosition.z == ChunkMetrics.chunkWidth) {
            Chunk neighbour = chunkManager.GetChunk(chunkPosition.x, chunkPosition.y + 1);
            
            if (neighbour) {
                // Update the edge at z = 0
                neighbour.Blocks[IndexFromCoord(localPosition.x, localPosition.y, 0)] = newBlockType;
                neighbour.Refresh();
            }
        }
    }

    int IndexFromCoord(int x, int y, int z) {
        return x + (ChunkMetrics.chunkWidth + 2) * (y + ChunkMetrics.chunkHeight * z);
    }

    public Vector3Int LocalPositionFromGlobal(Chunk chunk, Vector3 globalPosition) {
        int x = Mathf.FloorToInt(globalPosition.x - (ChunkMetrics.chunkWidth * chunk.Position.x));
        int y = Mathf.FloorToInt(globalPosition.y);
        int z = Mathf.FloorToInt(globalPosition.z - (ChunkMetrics.chunkWidth * chunk.Position.y));

        return new Vector3Int(x, y, z);
    }
}
