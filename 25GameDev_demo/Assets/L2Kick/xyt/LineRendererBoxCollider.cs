using UnityEngine;

public class LineRendererBoxColliders : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private BoxCollider[] boxColliders; // 用于存储所有的BoxCollider

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        InitializeBoxColliders();
    }

    void InitializeBoxColliders()
    {
        // 初始化BoxCollider数组
        boxColliders = new BoxCollider[lineRenderer.positionCount - 1];

        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 start = lineRenderer.GetPosition(i);
            Vector3 end = lineRenderer.GetPosition(i + 1);

            GameObject colliderObj = new GameObject("LineCollider");
            colliderObj.transform.parent = transform;
            colliderObj.transform.localScale = Vector3.one; // 确保缩放正确
            colliderObj.tag = gameObject.tag;

            // 添加BoxCollider
            BoxCollider boxCollider = colliderObj.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxColliders[i] = boxCollider; // 存储BoxCollider

            // 初始化BoxCollider的位置、大小和旋转
            UpdateBoxCollider(boxCollider, start, end);
        }
    }

    void Update()
    {
        UpdateBoxColliders();
    }

    void UpdateBoxColliders()
    {
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 start = lineRenderer.GetPosition(i);
            Vector3 end = lineRenderer.GetPosition(i + 1);

            // 更新BoxCollider的位置、大小和旋转
            UpdateBoxCollider(boxColliders[i], start, end);
        }
    }

    void UpdateBoxCollider(BoxCollider boxCollider, Vector3 start, Vector3 end)
    {
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider is null!");
            return;
        }

        // 计算线段的中点
        Vector3 midPoint = (start + end) / 2;
        boxCollider.transform.position = midPoint;

        // 计算线段的方向和长度
        Vector3 direction = (end - start).normalized;
        float lineLength = Vector3.Distance(start, end);

        // 设置BoxCollider的大小
        boxCollider.size = new Vector3(0.1f, lineLength, 0.1f);

        // 设置BoxCollider的旋转
        boxCollider.transform.rotation = Quaternion.LookRotation(direction);
        boxCollider.transform.Rotate(90f, 0f, 0f); // 根据需要调整旋转

    }
}