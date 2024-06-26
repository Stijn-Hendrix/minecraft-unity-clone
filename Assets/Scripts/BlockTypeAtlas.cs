﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block Type Atlas", menuName = "Custom/Create Block Type Atlas")]
public class BlockTypeAtlas : ScriptableObject
{
    public BlockType[] BlockTypes;

    public BlockTypeUvs[] BlockTypeUvs {
		get {
            BlockTypeUvs[] uvs = new BlockTypeUvs[BlockTypes.Length];

			for (int i = 0; i < BlockTypes.Length; i++) {
                uvs[i] = BlockTypes[i].BlockTypeUvs;
			}

            return uvs;
        }
	}
}

[System.Serializable]
public struct BlockType {
    public string Name;
    public BlockTypeUvs BlockTypeUvs;
    public Sprite inventorySprite;
}

[System.Serializable]
public struct BlockTypeUvs {
    public Vector2 TopUV;
    public Vector2 BottomUV;
    public Vector2 SidesUV;

    public static int SizeOf => sizeof(float) * 2 * 3;
}
