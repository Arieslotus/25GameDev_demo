using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2noteController : MonoBehaviour
{
    [Header("目标设置")]
    [HideInInspector]public Transform noteTarget; // 目标点
    [HideInInspector]public float circleRadius = 2f; // 圆圈的半径

    [Header("移动设置")]
    [HideInInspector]public float moveToAccTime; // 音符从生成点到圆圈边线的时间
    private float moveSpeed; // 计算出的移动速度

    private Vector3 startPosition; // 音符的生成位置
    [HideInInspector] public Vector3 targetPosition; // 音符的目标位置
    private Vector3 accuratePosition;
    private float journeyLength,accJourneyL; // 生成点到目标点的距离
    private float startTime; // 音符开始移动的时间

    public bool isMovingOutsideAcc = true;
    Rigidbody2D rig;
    bool flag = true;

    void Start()
    {
        if (noteTarget == null)
        {
            Debug.LogWarning("请设置目标对象！");
            return;
        }

        // 初始化
        startPosition = transform.position; // 记录生成位置
        accuratePosition = GetIntersectionPointOnCircle(startPosition, noteTarget.position); // 计算（圆圈边线上的点）交点
        journeyLength = Vector3.Distance(startPosition, targetPosition); // 计算距离
        accJourneyL = Vector3.Distance(startPosition, accuratePosition);

        // 根据移动时间和距离计算速度
        moveSpeed = accJourneyL / moveToAccTime;

        // 记录开始移动的时间
        startTime = Time.time;

        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(isMovingOutsideAcc)
        {
            // 计算已经移动的时间
            float distanceCovered = (Time.time - startTime) * moveSpeed;

            // 计算当前移动的进度（0 到 1 之间）
            float fractionOfAccurateJourney = distanceCovered / accJourneyL;

            // 使用 Lerp 平滑移动
            transform.position = Vector3.Lerp(startPosition, accuratePosition, fractionOfAccurateJourney);

            // 如果到达准确线内
            if (fractionOfAccurateJourney >= 1f)
            {
                isMovingOutsideAcc = false;
            }
        }
        else
        {
            // 计算已经移动的时间
            float distanceCovered = (Time.time - startTime) * moveSpeed;

            // 计算当前移动的进度（0 到 1 之间）
            float fractionOfJourney = distanceCovered / journeyLength;

            // 使用 Lerp 平滑移动
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
        }
        
        

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
    //private void OnDrawGizmosSelected()
    //{
    //    if (noteTarget != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(noteTarget.position, circleRadius);
    //    }
    //}

    //玩家点按
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);

        if (collision.CompareTag("CatBody"))
        {
            //小球打到猫身体(踹)
            Destroy(gameObject);
            //Debug.Log("bump body");
            //if (collision.GetComponent<SpriteRenderer>())
            //{
            //    collision.GetComponent<SpriteRenderer>().color = Color.red;
            //}
        }

        if (collision.CompareTag("CatLeg"))
        {
            //小球打到猫腿（扫腿）
            Destroy(gameObject);
            Debug.Log("bump leg");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "CatFoot")
        {
            //脚踢到小球（目前转速-200，不会出现玩家错过小球但仍然落在脚处的情况）
            Destroy(gameObject);
            //Debug.Log("bump foot");
        }
    }
}
