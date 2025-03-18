using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLine : MonoBehaviour
{
    public LineRenderer lineRenderer; // LineRenderer 组件
    public int pointsCount = 50; // 线条的点数
    public float amplitude = 0.5f; // 波浪的振幅
    public float frequency = 1f; // 波浪的频率
    public float waveLength = 2f; // 波浪的波长
    public float speed = 1f; // 波浪动画的速度

    private float timeOffset = 0f; // 时间偏移量，用于动画

    void Start()
    {
        // 初始化 LineRenderer
        lineRenderer.positionCount = pointsCount;
    }

    void Update()
    {
        // 更新时间偏移量
        timeOffset += Time.deltaTime * speed;

        // 生成波浪形状的点
        for (int i = 0; i < pointsCount; i++)
        {
            float x = (float)i / (pointsCount - 1) * waveLength; // X 坐标
            float y = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * x + timeOffset); // Y 坐标（波浪形状）
            lineRenderer.SetPosition(i, new Vector3(x, y, 0)); // 设置点
        }
    }
}
