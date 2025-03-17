using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2noteController : MonoBehaviour
{
    [Header("目标设置")]
    public Transform noteTarget; // 目标点
    public float circleRadius = 2f; // 圆圈的半径

    [Header("移动设置")]
    public float moveTime = 2f; // 音符从生成点到圆圈边线的时间
    private float moveSpeed; // 计算出的移动速度

    private Vector3 startPosition; // 音符的生成位置
    private Vector3 targetPosition; // 音符的目标位置（圆圈边线上的点）
    private float journeyLength; // 生成点到目标点的距离
    private float startTime; // 音符开始移动的时间

    void Start()
    {
        if (noteTarget == null)
        {
            Debug.LogWarning("请设置目标对象！");
            return;
        }

        // 初始化
        startPosition = transform.position; // 记录生成位置
        targetPosition = GetIntersectionPointOnCircle(startPosition, noteTarget.position); // 计算交点
        journeyLength = Vector3.Distance(startPosition, targetPosition); // 计算距离

        // 根据移动时间和距离计算速度
        moveSpeed = journeyLength / moveTime;

        // 记录开始移动的时间
        startTime = Time.time;
    }

    void Update()
    {
        if (noteTarget == null) return;

        // 计算已经移动的时间
        float distanceCovered = (Time.time - startTime) * moveSpeed;

        // 计算当前移动的进度（0 到 1 之间）
        float fractionOfJourney = distanceCovered / journeyLength;

        // 使用 Lerp 平滑移动
        transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

        //// 如果到达目标点，销毁音符
        //if (fractionOfJourney >= 1f)
        //{
        //    Destroy(gameObject);
        //}
    }

    // 计算生成点到目标点的连线与圆圈边线的交点
    private Vector3 GetIntersectionPointOnCircle(Vector3 start, Vector3 end)
    {
        // 计算方向向量
        Vector3 direction = (start - end).normalized;

        // 计算交点
        Vector3 intersectionPoint = noteTarget.position + direction * circleRadius;

        return intersectionPoint;
    }

    // 在 Scene 视图中绘制圆圈（方便调试）
    private void OnDrawGizmosSelected()
    {
        if (noteTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(noteTarget.position, circleRadius);
        }
    }

    //玩家点按
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("1");
        if (other.CompareTag("Player"))
        {
            Debug.Log("2");
            Destroy(gameObject);
        }
    }
}
