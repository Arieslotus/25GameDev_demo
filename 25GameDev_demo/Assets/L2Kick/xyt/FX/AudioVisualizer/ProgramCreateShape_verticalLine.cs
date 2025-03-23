using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 程序创建Shape， u 和 v来确定每个点的xyz
/// </summary>
public class ProgramCreateShape_verticalLine : MonoBehaviour
{
    public GameObject pointPrefab;

    [Range(10, 100)]
    public int cubeNum = 10;

    [Range(0.1f, 5f)]
    public float height = 1f; // ➊ 添加可调节的半径
    public Transform middlePoint;
    public bool isReverse = false;

    public LineRenderer line2;
    private LineRenderer line;

    Transform[] points;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        float step = 10f / cubeNum;
        Vector3 scale = Vector3.one * step;

        points = new Transform[cubeNum];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab).transform;
            point.localScale = scale;
            point.SetParent(this.transform, false);
            points[i] = point;
        }

    }

    private void Start()
    {
        int group = 1;
        float step = 2f / cubeNum;

        for (int x = 0; x < cubeNum; x++)
        {
            float u = (x + 0.5f) * step - 1f;
            points[x].localPosition = VerticalLine(middlePoint.position,u, height , isReverse); // ➋ 传入 radius
            points[x].GetComponent<VisibleGroup_verticalLine>().groupIndex = group;

            if(isReverse)
            {
                points[x].GetComponent<VisibleGroup_verticalLine>().isOnRight = false;
            }
            else
            {
                points[x].GetComponent<VisibleGroup_verticalLine>().isOnRight = true;
            }

            group++;
        }
    }

    private void LateUpdate()
    {
        //for (int i = 0; i < cubeNum; i++)
        //{
        //    line.SetPosition(i, points[i].GetComponent<VisibleGroup_verticalLine>().link1.position);
        //}
        //line.SetPosition(cubeNum, points[0].GetComponent<VisibleGroup_verticalLine>().link1.position);

        //for (int i = 0; i < cubeNum; i++)
        //{
        //    line2.SetPosition(i, points[i].GetComponent<VisibleGroup_verticalLine>().link2.position);
        //}
        //line2.SetPosition(cubeNum, points[0].GetComponent<VisibleGroup_verticalLine>().link2.position);
    
    }

    const float pi = Mathf.PI;

    /// <summary>
    /// 生成一系列竖直方向上等距排布的位置坐标
    /// </summary>
    /// <param name="v">归一化的位置参数 (-1 到 1 之间)</param>
    /// <param name="height">最大高度</param>
    /// <returns>竖直方向的 Vector3 坐标</returns>
    static Vector3 VerticalLine(Vector3 middlePos , float v, float height , bool isReverse)
    {
        Vector3 p;
        p.x = middlePos.x + 0; // 所有点的 X 轴坐标相同
        if(isReverse)
        {
            p.y = middlePos.y + v * (-height); // Y 轴等距分布
        }
        else
        {
            p.y = middlePos.y + v * height; // Y 轴等距分布
        }
        
        p.z = 0; // 可以扩展为3D
        return p;
    }


}
