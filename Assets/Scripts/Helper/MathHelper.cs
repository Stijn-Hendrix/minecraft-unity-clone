using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    public static int ManhattanDistance(Vector3Int a, Vector3Int b) {
        checked {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }

    public static int ManhattanDistance(this Vector2Int a, Vector2Int b) {
        checked {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}
