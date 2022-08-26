using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkEdit : MonoBehaviour
{
    [Header("--- References ---")]
	public Camera cam;
    public ChunkManager chunkManager;
    public BlockHighlight blockHighlight;
    public Inventory inventory;

    [Header("--- Animations ---")]
    public Animator handHandimator;

    [Header("--- Editing ---")]
    public CharacterController playerController;
    public float editDistance = 4f;
    public LayerMask editLayerMask;



    float addBlockCooldown = 0f;
    float removeBlockCooldown = 0f;

    const float timeToRemoveBlock = 1f;

    Vector3 blockHighlightPosition;
    Vector3 lastBlockHighlightPosition;


    private void Update() {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, editDistance, editLayerMask)) {
            blockHighlightPosition = hit.point + cam.transform.forward * 0.01f;

            blockHighlightPosition.x = Mathf.FloorToInt(blockHighlightPosition.x) + 0.5f;
            blockHighlightPosition.y = Mathf.FloorToInt(blockHighlightPosition.y) + 0.5f;
            blockHighlightPosition.z = Mathf.FloorToInt(blockHighlightPosition.z) + 0.5f;

            blockHighlight.Visible = true;
            blockHighlight.transform.position = blockHighlightPosition;

            if (lastBlockHighlightPosition != blockHighlightPosition) {
                removeBlockCooldown = 0;
            }

            lastBlockHighlightPosition = blockHighlightPosition;
        }
        else {
            blockHighlight.Visible = false;
        }
      

        if (Input.GetKey(KeyCode.Mouse0)) {
            if (blockHighlight.Visible) {
                blockHighlight.BreakVisible = true;

                removeBlockCooldown += Time.deltaTime;

                float progress = removeBlockCooldown / timeToRemoveBlock * 100;

                blockHighlight.SetProgress(progress);

                if (removeBlockCooldown >= timeToRemoveBlock) {
                    RemoveBlock();
                    removeBlockCooldown = 0;
                }
            }
            else {
                blockHighlight.BreakVisible = false;
                removeBlockCooldown = 0;
            }
        }
        else {
            blockHighlight.BreakVisible = false;
            removeBlockCooldown = 0;
        }

      
        if (Input.GetKey(KeyCode.Mouse1)) {
            addBlockCooldown -= Time.deltaTime * 15;

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
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, editDistance, editLayerMask)) {
            Vector3 offsetPosition = hit.point + cam.transform.forward * 0.01f;

            Vector2Int offsetChunkPosition = ChunkManager.ChunkFromWorldPosition(offsetPosition.x, offsetPosition.z);
            Chunk offsetChunk = chunkManager.GetChunk(offsetChunkPosition.x, offsetChunkPosition.y);

            Vector3Int localPosition = ChunkManager.LocalPositionFromWorldPosition(offsetChunk, offsetPosition);

            int replacedType = EditChunk(offsetChunk, localPosition, 0);
            inventory.Add(replacedType);
        }
    }

    void AddBlock() {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, editDistance, editLayerMask)) {
            Vector3 offsetPosition = hit.point - cam.transform.forward * 0.01f;


            Vector2Int offsetChunkPosition = ChunkManager.ChunkFromWorldPosition(offsetPosition.x, offsetPosition.z);
            Chunk offsetChunk = chunkManager.GetChunk(offsetChunkPosition.x, offsetChunkPosition.y);

            Vector3Int localPosition = ChunkManager.LocalPositionFromWorldPosition(offsetChunk, offsetPosition);

            Bounds blockBound = new Bounds(offsetPosition, Vector3.one * 1.5f);
            if (playerController.bounds.Intersects(blockBound)) {
                return;
            }

            int selectedBlock = inventory.RemoveSelectedBlock();

            if (selectedBlock == 0) {
                return;
			}

            EditChunk(offsetChunk, localPosition, selectedBlock);
        }
    }

    int EditChunk(Chunk chunk, Vector3Int localPosition, int newBlockType = 0) {
        int blockTypeToReplace = chunk.Blocks[IndexFromCoord(localPosition.x, localPosition.y, localPosition.z)] ;
        if (blockTypeToReplace == newBlockType) {
            return 0;
		}
        chunk.Blocks[IndexFromCoord(localPosition.x, localPosition.y, localPosition.z)] = newBlockType;
        chunk.Refresh();
        chunk.HasBeenEdited = true;

        Vector2Int chunkPosition = chunk.Position;

        if (localPosition.x == 1) {
            Chunk neighbour = chunkManager.GetChunk(chunkPosition.x - 1, chunkPosition.y);

            if (neighbour) {
                // Update the edge at x = chunkWidth + 1
                neighbour.Blocks[IndexFromCoord(ChunkMetrics.chunkWidth + 1, localPosition.y, localPosition.z)] = newBlockType;
                neighbour.Refresh();
                neighbour.HasBeenEdited = true;
            }
        }


        if (localPosition.z == 1) {
            Chunk neighbour = chunkManager.GetChunk(chunkPosition.x, chunkPosition.y - 1);

            if (neighbour) {
                // Update the edge at z = chunkWidth + 1
                neighbour.Blocks[IndexFromCoord(localPosition.x, localPosition.y, ChunkMetrics.chunkWidth + 1)] = newBlockType;
                neighbour.Refresh();
                neighbour.HasBeenEdited = true;
            }
        }

        if (localPosition.x == ChunkMetrics.chunkWidth) {
            Chunk neighbour = chunkManager.GetChunk(chunkPosition.x + 1, chunkPosition.y);

            if (neighbour) {
                // Update the edge at x = 0
                neighbour.Blocks[IndexFromCoord(0, localPosition.y, localPosition.z)] = newBlockType;
                neighbour.Refresh();
                neighbour.HasBeenEdited = true;
            }

        }

        if (localPosition.z == ChunkMetrics.chunkWidth) {
            Chunk neighbour = chunkManager.GetChunk(chunkPosition.x, chunkPosition.y + 1);
            
            if (neighbour) {
                // Update the edge at z = 0
                neighbour.Blocks[IndexFromCoord(localPosition.x, localPosition.y, 0)] = newBlockType;
                neighbour.Refresh();
                neighbour.HasBeenEdited = true;
            }
        }
        return blockTypeToReplace;
    }

    int IndexFromCoord(int x, int y, int z) {
        return x + (ChunkMetrics.chunkWidth + 2) * (y + ChunkMetrics.chunkHeight * z);
    }

}
