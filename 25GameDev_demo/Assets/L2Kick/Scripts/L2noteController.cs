using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    float myTime = 0;
    [HideInInspector] public float checkRange;
    [HideInInspector] public float perfectCheckRange;
    bool hadAdd = false, hadRemove = false;

    [Header("表现设置")]
    public float rotationSpeed = 50f;
    // 缩放速度
    public float scaleSpeed = 0.5f;
    // 缩放范围（最小和最大缩放比例）
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    private Vector3 initialScale; // 初始缩放值
    private float currentScaleFactor = 1f; // 当前缩放因子
    private bool scalingUp = true; // 是否正在放大

    private void Awake()
    {
        L2gameController gameController = FindObjectOfType<L2gameController>();
        noteTarget = gameController.noteTarget;
        circleRadius = gameController.accCircleRadius;
        moveToAccTime = gameController.moveToAccTime;
        targetPosition = gameController.cat.transform.position;
        checkRange = gameController.checkTimeRange;
        perfectCheckRange = gameController.perfectCheckTimeRange;
    }

    void Start()
    {
        //Debug.Log("checkrange" + checkRange);
        initialScale = transform.localScale;

        myTime = - FindObjectOfType<L2gameController>().moveToAccTime;
        //Debug.Log("myTime" + myTime);
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
        myTime += Time.deltaTime;

        RotateObject();
        ScaleObject();

        //Debug.Log("mytime" + myTime);
        //进入判定区间
        if (!hadAdd && myTime > -checkRange / 2)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            //Debug.Log("tap1");
            L2CheckList.tapCheckList.Add(this);
            hadAdd = true;
        }
        else if (!hadRemove && myTime > checkRange/2) //出判定区间
        {
            L2CheckList.tapCheckList.Remove(this);
            hadRemove = true;
            Miss();
        }

        if (isMovingOutsideAcc)
        {
            // 计算已经移动的时间
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            // 计算当前移动的进度（0 到 1 之间）
            float fractionOfAccurateJourney = distanceCovered / accJourneyL;

            if (fractionOfAccurateJourney < 1f)
            {
                transform.position = Vector3.Lerp(startPosition, accuratePosition, fractionOfAccurateJourney);
            }
            else
            {
                // 到达 accuratePosition，开始第二阶段
                isMovingOutsideAcc = false;
                //startTime = Time.time; // 重置开始时间
            }
        }
        else
        {
            // 计算已经移动的时间
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            // 计算当前移动的进度（0 到 1 之间）
            float fractionOfTargetJourney = distanceCovered / journeyLength;

            if (fractionOfTargetJourney < 1f)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfTargetJourney);
            }
            else
            {
                transform.position = targetPosition;
            }
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

    public bool CheckNote_Tap()
    {
        Debug.Log("tap");
        FindObjectOfType<L2gameController>().JudgeNote(myTime);
        L2CheckList.tapCheckList.Remove(this);
        Destroy(gameObject);
        return true;
    }

    void Miss()
    {
        Debug.Log("miss");
        //特效
        //积分
        FindObjectOfType<L2gameController>().MissNote();
        L2CheckList.tapCheckList.Remove(this);
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        Destroy(gameObject);
    }

    public void showCheckResult(int x)
    {
        //1perfect 2good 0miss

    }

    private void RotateObject()
    {
        // 绕 Z 轴旋转
        transform.Rotate(0,0, rotationSpeed * Time.deltaTime);
    }

    private void ScaleObject()
    {
        // 计算缩放因子
        if (scalingUp)
        {
            currentScaleFactor += scaleSpeed * Time.deltaTime;
            if (currentScaleFactor >= maxScale)
            {
                currentScaleFactor = maxScale;
                scalingUp = false;
            }
        }
        else
        {
            currentScaleFactor -= scaleSpeed * Time.deltaTime;
            if (currentScaleFactor <= minScale)
            {
                currentScaleFactor = minScale;
                scalingUp = true;
            }
        }

        // 应用缩放
        transform.localScale = initialScale * currentScaleFactor;
    }
}
