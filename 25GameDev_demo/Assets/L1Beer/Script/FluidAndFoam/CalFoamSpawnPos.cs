using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalFoamSpawnPos : MonoBehaviour
{
    public List<Transform> targetRangesList = new List<Transform>();
    void Start()
    {
        targetRangesList = GetAllChildren(transform);
    }


    public Vector3 CalPos()
    {
        int i = Random.Range(0, targetRangesList.Count);

        // 获取目标物体的渲染区域（假设目标物体有一个 SpriteRenderer 或 MeshRenderer）
        Renderer targetRenderer = targetRangesList[i].GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogError("Target object does not have a Renderer component.");
            return Vector3.zero;  // 返回一个默认值
        }

        // 获取目标物体的边界框（world space bounds）
        Bounds bounds = targetRenderer.bounds;

        // 获取目标物体的中心位置和大小
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        // 在 X 和 Y 方向分别生成一个随机偏移量，范围在宽度和高度的一半内
        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomY = Random.Range(center.y - size.y / 2, center.y + size.y / 2);

        // 返回生成的随机位置
        return new Vector3(randomX, randomY, center.z);  // Z 坐标保持不变

    }




    List<Transform> GetAllChildren(Transform parent)
    {
        List<Transform> children = new List<Transform>();

        // 遍历父物体的所有子物体
        foreach (Transform child in parent)
        {
            children.Add(child);
            // 如果子物体也有子物体，则递归调用
            children.AddRange(GetAllChildren(child));
        }

        return children;
    }
}
