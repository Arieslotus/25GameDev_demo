using UnityEngine;

public class LineRendererBoxColliders : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private BoxCollider[] boxColliders; // ���ڴ洢���е�BoxCollider

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        InitializeBoxColliders();
    }

    void InitializeBoxColliders()
    {
        // ��ʼ��BoxCollider����
        boxColliders = new BoxCollider[lineRenderer.positionCount - 1];

        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 start = lineRenderer.GetPosition(i);
            Vector3 end = lineRenderer.GetPosition(i + 1);

            GameObject colliderObj = new GameObject("LineCollider");
            colliderObj.transform.parent = transform;
            colliderObj.transform.localScale = Vector3.one; // ȷ��������ȷ
            colliderObj.tag = gameObject.tag;

            // ���BoxCollider
            BoxCollider boxCollider = colliderObj.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxColliders[i] = boxCollider; // �洢BoxCollider

            // ��ʼ��BoxCollider��λ�á���С����ת
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

            // ����BoxCollider��λ�á���С����ת
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

        // �����߶ε��е�
        Vector3 midPoint = (start + end) / 2;
        boxCollider.transform.position = midPoint;

        // �����߶εķ���ͳ���
        Vector3 direction = (end - start).normalized;
        float lineLength = Vector3.Distance(start, end);

        // ����BoxCollider�Ĵ�С
        boxCollider.size = new Vector3(0.1f, lineLength, 0.1f);

        // ����BoxCollider����ת
        boxCollider.transform.rotation = Quaternion.LookRotation(direction);
        boxCollider.transform.Rotate(90f, 0f, 0f); // ������Ҫ������ת

    }
}