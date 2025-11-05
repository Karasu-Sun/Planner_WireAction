using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTilePlacer : MonoBehaviour
{
    [Header("配置設定")]
    public GameObject prefab; // プレハブ
    public Vector3 startPoint; // 開始地点
    public Vector3 endPoint; // 終了地点
    public Vector3 spacing = new Vector3(1f, 1f, 1f); // 配置間隔
    public bool includeEdges = true; // 端を含めるかどうか
    public bool destroyOld = true; // 既存オブジェクトを破棄するか
    [ContextMenu("Place Prefabs")]
    public void PlacePrefabs()
    {
        if (prefab == null)
        {
            Debug.LogWarning("Prefabが設定されていません。");
            return;
        }

        // 既存子オブジェクトの破棄
        if (destroyOld)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        Vector3 min = Vector3.Min(startPoint, endPoint);
        Vector3 max = Vector3.Max(startPoint, endPoint);

        // 配置数計算
        int xCount = Mathf.FloorToInt((max.x - min.x) / spacing.x) + (includeEdges ? 1 : 0);
        int yCount = Mathf.FloorToInt((max.y - min.y) / spacing.y) + (includeEdges ? 1 : 0);
        int zCount = Mathf.FloorToInt((max.z - min.z) / spacing.z) + (includeEdges ? 1 : 0);

        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // 配置ループ
        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                for (int z = 0; z < zCount; z++)
                {
                    Vector3 pos = new Vector3(
                        min.x + x * spacing.x,
                        min.y + y * spacing.y,
                        min.z + z * spacing.z
                    );

                    GameObject obj = Instantiate(prefab, pos, Quaternion.identity, transform);
                    obj.name = $"Prefab_{x}_{y}_{z}";
                }
            }
        }

        Debug.Log($"合計 {xCount * yCount * zCount} 個のプレハブを配置しました。");
    }
}