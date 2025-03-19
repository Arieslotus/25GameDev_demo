using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLine : MonoBehaviour
{
    public LineRenderer lineRenderer; // LineRenderer ���
    public int pointsCount = 50; // �����ĵ���
    public float amplitude = 0.5f; // ���˵����
    public float frequency = 1f; // ���˵�Ƶ��
    public float waveLength = 2f; // ���˵Ĳ���
    public float speed = 1f; // ���˶������ٶ�

    private float timeOffset = 0f; // ʱ��ƫ���������ڶ���

    void Start()
    {
        // ��ʼ�� LineRenderer
        lineRenderer.positionCount = pointsCount;
    }

    void Update()
    {
        // ����ʱ��ƫ����
        timeOffset += Time.deltaTime * speed;

        // ���ɲ�����״�ĵ�
        for (int i = 0; i < pointsCount; i++)
        {
            float x = (float)i / (pointsCount - 1) * waveLength; // X ����
            float y = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * x + timeOffset); // Y ���꣨������״��
            lineRenderer.SetPosition(i, new Vector3(x, y, 0)); // ���õ�
        }
    }
}
