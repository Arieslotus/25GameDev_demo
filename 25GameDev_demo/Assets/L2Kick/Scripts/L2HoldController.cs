using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public enum footStatus
{
    perfect,
    good,
    none
}
public class L2HoldController : MonoBehaviour
{
    [Header("目标设置")]
    [HideInInspector] public Transform noteTarget; // 目标点
    [HideInInspector] public float circleRadius = 2f; // 圆圈的半径

    [Header("移动设置")]
    [HideInInspector] public float moveToAccTime; // 音符从生成点到圆圈边线的时间
    private float moveSpeed; // 计算出的移动速度

    private Vector3 startPosition; // 音符的生成位置
    [HideInInspector] public Vector3 targetPosition; // 音符的目标位置
    private Vector3 accuratePosition;
    private float journeyLength, accJourneyL; // 生成点到目标点的距离
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
    public float scaleSpeed = 0.5f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float targetAlpha = 0.5f;

    private Vector3 initialScale; // 初始缩放值
    private float currentScaleFactor = 1f; // 当前缩放因子
    private bool scalingUp = true; // 是否正在放大

    //hold
    [HideInInspector]
    public bool isFirst = false;
    public float holdTime;
    bool isHolding = false;
    float isHoldingTime;
    float hitTime;
    bool isFirstFrame = true;
    public bool isSpaceReleased = false; // 是否松开了空格
    private Coroutine holdingCoroutine; // 用于存储协程引用
    bool hadSetAlpha = false;
    //holdrow
    //bool L2CheckList.headHadMiss = false;

    [Header("特效")]
    public GameObject perfectHoldPre;
    public GameObject goodHoldPre;
    public GameObject smokePre;
    GameObject perfectHold;
    GameObject goodHold;
    GameObject smoke;
    public ParticleSystem[] perfectHoldEffect;
    public ParticleSystem[] goodHoldEffect;
    public ParticleSystem[] smokeEffect;
    private GameObject catRFoot, catLFoot;


    public footStatus myFootStatus;


    private void Awake()
    {
        gameObject.SetActive(true);
        L2gameController gameController = FindObjectOfType<L2gameController>();
        noteTarget = gameController.noteTarget;
        circleRadius = gameController.accCircleRadius;
        moveToAccTime = gameController.moveToAccTime;
        targetPosition = gameController.cat.transform.position;
        checkRange = gameController.checkTimeRange;
        perfectCheckRange = gameController.perfectCheckTimeRange;
    }

    private void OnEnable()
    {
        
    }

    void Start()
    {
        //foot
        catRFoot = GameObject.Find("footRSprite");
        catLFoot = GameObject.Find("footLSprite");
        myFootStatus = footStatus.none;

        //goodHoldEffect= goodHold.GetComponentsInChildren<ParticleSystem>();


        //Debug.Log("checkrange" + checkRange);
        initialScale = transform.localScale;

        myTime = -FindObjectOfType<L2gameController>().moveToAccTime;
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

        if (!isFirst)
        {
            gameObject.transform.localScale *= 0.6f;
        }
        else
        {
            //L2CheckList.headHadMiss = false;
            //L2CheckList.isSpaceReleased = false;
        }
    }

    void Update()
    {
        myTime += Time.deltaTime;

        //effect pos
        if (isFirst)
        {
            var cat = FindObjectOfType<CatController>();
            Vector3 footpos = Vector3.zero;
            if (cat.isOutLFoot)
            {
                footpos = catLFoot.GetComponent<Transform>().position;
            }
            else if (cat.isOutRFoot)
            {
                footpos = catRFoot.GetComponent<Transform>().position;
            }

           // EffectsAtPosition(footpos);
        }

        if (isFirst)
        {
            RotateObject();
            ScaleObject();

            //Debug.Log("mytime" + myTime);
        }

        //Debug.Log("mytime" + myTime);
        //进入判定区间
        
        if ( myTime > -checkRange / 2 && myTime < checkRange / 2)
        {
            //Debug.Log("进范围");
            if (!hadAdd)//执行一次
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

                if (isFirst)
                {
                    L2CheckList.headCheckList.Add(this);
                    hitTime = myTime;
                }
                else
                {
                    L2CheckList.holdRow.Add(this);
                }
                hadAdd = true;
            }
            else //循环
            {
                if (isFirst)
                {
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        // 中途松开空格，标记为松开
                        isSpaceReleased = true;
                        myFootStatus = footStatus.none;
                        //L2CheckList.isSpaceReleased = true;

                    }
                }
                else
                {
                    if (L2CheckList.headHadMiss && !hadSetAlpha)
                    {
                        //淡出
                        //Debug.Log("淡出");
                        StopPerfectEffect();
                        Color color = GetComponent<SpriteRenderer>().color;
                        color.a = 0.1f;
                        GetComponent<SpriteRenderer>().color = color;
                        hadSetAlpha = true;
                    }
                }
            }
        }
        else if (myTime > checkRange / 2) //出判定区间,撞到猫
        {
            //Debug.Log("出范围");
            if (!hadRemove)
            {
                //Debug.Log("出范围");
                if (isFirst)
                {
                    //Debug.Log("miss");
                    Miss();
                }
                else
                {
                    L2CheckList.holdRow.Remove(this);
                    Destroy(gameObject);
                }
            }
            hadRemove = true;
        }
        
            //move
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

                //玩家正常hold的destroy逻辑
                if (!isFirst && !L2CheckList.headHadMiss && Input.GetKey(KeyCode.Space))
                {
                    if(L2CheckList.headCheckList.Count>0 && !L2CheckList.headCheckList[0].isSpaceReleased)
                    {
                        //Debug.Log("hold:destroy");
                        L2CheckList.holdRow.Remove(this);
                        Destroy(gameObject);
                    }

                }
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

    public bool CheckNote_Head() //已经按下
    {
        hadRemove = true;
        isHolding = true;
        L2CheckList.headHadMiss = false;
        myFootStatus = footStatus.perfect;
        //命中特效
        //PlayPerfectEffect();
        FindObjectOfType<L2gameController>().JudgeNote(hitTime); // 传入判定时间
        if (this != null ) // 检查对象是否被销毁!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!?????????????????????????
            holdingCoroutine = StartCoroutine(HoldingTimer());
        return true;
    }


    IEnumerator HoldingTimer()
    {
        float elapsedTime = 0f;
        while (elapsedTime < holdTime)
        {
            if (this == null || gameObject == null) // 检查对象是否被销毁
            {
                yield break; // 如果对象被销毁，停止协程
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }
        if (this == null || gameObject == null) // 检查对象是否被销毁
        {
            yield break; // 如果对象被销毁，停止协程
        }
        // Hold 完成
        if (isSpaceReleased)
        {
            // 如果松开过空格，触发 Miss
            Miss();
        }
        else
        {
            Debug.Log("finish+stop effect");
            myFootStatus = footStatus.none;
            StopPerfectEffect();
            L2CheckList.headCheckList.Remove(this); // 从 holdCheckList 中移除
            Destroy(gameObject);
        }
    }

    void Miss()
    {
        L2CheckList.headHadMiss = true;
        //Debug.Log("L2CheckList.headHadMiss" + L2CheckList.headHadMiss);
        //Debug.Log("miss");
        //特效
        //积分
        myFootStatus = footStatus.none;
        //StopPerfectEffect();
        FindObjectOfType<L2gameController>().MissNote();
        L2CheckList.headCheckList.Remove(this);
        Destroy(gameObject);
    }


    private void RotateObject()
    {
        // 绕 Z 轴旋转
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
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

    void OnDestroy()
    {
        if (isFirst)
            Debug.Log("firstDestroyed");
    }

    void PlayPerfectEffect()
    {
        Debug.Log("ins hold effect");
        perfectHold = Instantiate(perfectHoldPre, transform.position, Quaternion.identity);
        perfectHoldEffect = perfectHold.GetComponentsInChildren<ParticleSystem>();
        smoke = Instantiate(smokePre, transform.position, Quaternion.identity);
        smokeEffect = smoke.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in smokeEffect)
        {
            ps.Play();
        }
        foreach (ParticleSystem ps in perfectHoldEffect)
        {
            ps.Play();
        }
        //Destroy(smoke, 2f);
    }

    void StopPerfectEffect()
    {
        Debug.Log("stop effect");
        foreach (ParticleSystem ps in perfectHoldEffect)
        {
            ps.Stop();
        }
        Destroy(perfectHold);
    }

    void EffectsAtPosition(Vector3 position)
    {
        if (perfectHold != null)
        {
            foreach (ParticleSystem ps in perfectHoldEffect)
            {
                // 设置粒子系统位置
                ps.transform.position = position;
            }
        }
    }
}
