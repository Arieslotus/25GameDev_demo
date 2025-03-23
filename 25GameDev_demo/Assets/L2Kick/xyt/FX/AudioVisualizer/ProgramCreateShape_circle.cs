using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CircleType
{
    BothJump,
    OutlineJump,
    InlineJump
}
public enum CircleShow
{
    Whole,
    Quarter1,
    Quarter3,
}



/// <summary>
/// 程序创建Shape， u 和 v来确定每个点的xyz
/// </summary>
public class ProgramCreateShape_circle : MonoBehaviour
{
    public GameObject pointPrefab;

    [Range(10, 200)]
    public int cubeNum = 10;

    [Range(0.1f, 5f)]
    public float radius = 1f; // ➊ 添加可调节的半径
    public Transform middlePoint;
    public CircleType circleType;
    public CircleShow circleShow = CircleShow.Whole;
    public bool drawConnectLines = true;


    public LineRenderer line2;
    private LineRenderer line;

    Transform[] points;

    bool flag = false;

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
            points[x].localPosition = Cylinder(middlePoint.position,u, radius); // ➋ 传入 radius
            points[x].GetComponent<VisibleGroup>().groupIndex = group;
            points[x].GetComponent<VisibleGroup>().circleType = circleType;

            switch (circleShow)
            {
                case CircleShow.Whole:
                    points[x].GetComponent<LinkGroup>().show = true;
                    break;
                case CircleShow.Quarter1:
                    if (x <= cubeNum / 4)
                    {
                        points[x].GetComponent<LinkGroup>().show = true;
                    }
                    else
                    {
                        points[x].GetComponent<LinkGroup>().show = false;
                    }
                    break;
                case CircleShow.Quarter3:
                    if (2*cubeNum/4 <= x && x <= 3*cubeNum /4)
                    {
                        points[x].GetComponent<LinkGroup>().show = true;
                    }
                    else
                    {
                        points[x].GetComponent<LinkGroup>().show = false;
                    }
                    break;
            }
            

            if (group >= cubeNum / 2 && !flag)
            {
                group = 0;
                flag = true;
            }

            group++;
        }
    }

    private void LateUpdate()
    {
        if(!drawConnectLines) return;

        
        for (int i = 0; i < cubeNum; i++)
        {
            switch (circleShow)
            {
                case CircleShow .Whole:
                    line.SetPosition(i, points[i].GetComponent<VisibleGroup>().link1.position);
                    
                    break;
                case CircleShow .Quarter1:
                    if(i <= cubeNum / 4)
                    {
                        //line.SetPosition(i, points[i].GetComponent<VisibleGroup>().link1.position);
                    }
                    else
                    {
                        //line.SetPosition(i, points[cubeNum / 4].GetComponent<VisibleGroup>().link1.position);
                    }

                    break; 
                case CircleShow .Quarter3:
                    if (2 * cubeNum / 4 <= i && i <= 3 * cubeNum / 4)
                    {
                        //line.SetPosition(i, points[i].GetComponent<VisibleGroup>().link1.position);
                    }
                    else
                    {

                    }
                    break;
            }
            
        }
        if(circleShow == CircleShow .Whole)
        {
            line.SetPosition(cubeNum, points[0].GetComponent<VisibleGroup>().link1.position);
        }
        else if(circleShow == CircleShow .Quarter1)
        {
            //line.SetPosition(cubeNum, points[cubeNum/4].GetComponent<VisibleGroup>().link1.position);
        }
        else
        {
            //line.SetPosition(cubeNum, points[3*cubeNum / 4].GetComponent<VisibleGroup>().link1.position);
        }
        

        for (int i = 0; i < cubeNum; i++)
        {
            line2.SetPosition(i, points[i].GetComponent<VisibleGroup>().link2.position);
        }
        line2.SetPosition(cubeNum, points[0].GetComponent<VisibleGroup>().link2.position);
    }

    const float pi = Mathf.PI;

    /// <summary>
    /// 计算圆的坐标，并加入可调节的半径
    /// </summary>
    static Vector3 Cylinder(Vector3 middlePos,float u, float radius) // ➌ 添加 radius 参数
    {
        Vector3 p;
        p.x = middlePos.x + Mathf.Sin(pi * u) * radius; // 乘以 radius
        p.y = middlePos.y + Mathf.Cos(pi * u) * radius; // 乘以 radius
        p.z = 0;
        return p;
    }


}
