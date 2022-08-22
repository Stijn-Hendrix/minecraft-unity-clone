using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkEdit : MonoBehaviour
{
	public Camera cam;
    public ChunkManager chunkManager;

    public float editDistance = 4f;
    public LayerMask editLayerMask;

    public CharacterController controller;

    public Animator handHandimator;

    public GameObject highlight;

    float addBlockCooldown = 0f;
    float removeBlockCooldown = 0f;


	private void Update() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, editDistance, editLayerMask)) {
                Vector3 offsetPosition = hit.point - cam.transform.forward * 0.01f;

                offsetPosition.x = Mathf.FloorToInt(offsetPosition.x) + 0.5f;
                offsetPosition.y = Mathf.FloorToInt(offsetPosition.y) + 0.5f;
                offsetPosition.z = Mathf.FloorToInt(offsetPosition.z) + 0.5f;

                highlight.SetActive(true);
                highlight.transform.position = offsetPosition;
            }
            else {
                highlight.SetActive(false);
            }
        }
        else {
            highlight.SetActive(false);
        }

        if (Input.GetKey(KeyCode.Mouse0)) {
            removeBlockCooldown -= Time.deltaTime * 10;

            if (removeBlockCooldown <= 0) {
                RemoveBlock();
                removeBlockCooldown = 1f;
            }
        }

      
        if (Input.GetKey(KeyCode.Mouse1)) {
            addBlockCooldown -= Time.deltaTime * 10;

            if (addBlockCooldown <= 0) {
                AddBlock();
                addBlockCooldown = 1f;
			}

        }

        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1)) {
            handHandimator.Play("Punch");
        }
    }

    void RemoveBlock() {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, editDistance, editLayerMask)) {
            Vector3 offsetPosition = hit.point + cam.transform.forward * 0.01f;

            Vector2Int offsetChunkPosition = ChunkManager.ChunkFromWorldPosition(offsetPosition.x, offsetPosition.z);
            Chunk offsetChunk = chunkManager.GetChunk(offsetChunkPosition.x, offsetChunkPosition.y);

            Vector3Int localPosition = ChunkManager.LocalPositionFromWorldPosition(offsetChunk, offsetPosition);

            EditChunk(offsetChunk, localPosition, 0);
        }
    }

    void AddBlock() {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, editDistance, editLayerMask)) {
            Vector3 offsetPosition = hit.point - cam.transform.forward * 0.01f;


            Vector2Int offsetChunkPosition = ChunkManager.ChunkFromWorldPosition(offsetPosition.x, offsetPosition.z);
            Chunk offsetChunk = chunkManager.GetChunk(offsetChunkPosition.x, offsetChunkPosition.y);

            Vector3Int localPosition = ChunkManager.LocalPositionFromWorldPosition(offsetChunk, offsetPosition);

            Bounds blockBound = new Bounds(offsetPosition, Vector3.one * 1.5f);
            if (controller.bounds.Intersects(blockBound)) {
                return;
            }

            EditChunk(offsetChunk, localPosition, 1);
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

}
