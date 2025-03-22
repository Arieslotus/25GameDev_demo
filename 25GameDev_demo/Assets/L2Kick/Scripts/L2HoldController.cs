using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2HoldController : MonoBehaviour
{
    [Header("Ŀ������")]
    [HideInInspector] public Transform noteTarget; // Ŀ���
    [HideInInspector] public float circleRadius = 2f; // ԲȦ�İ뾶

    [Header("�ƶ�����")]
    [HideInInspector] public float moveToAccTime; // ���������ɵ㵽ԲȦ���ߵ�ʱ��
    private float moveSpeed; // ��������ƶ��ٶ�

    private Vector3 startPosition; // ����������λ��
    [HideInInspector] public Vector3 targetPosition; // ������Ŀ��λ��
    private Vector3 accuratePosition;
    private float journeyLength, accJourneyL; // ���ɵ㵽Ŀ���ľ���
    private float startTime; // ������ʼ�ƶ���ʱ��

    public bool isMovingOutsideAcc = true;
    Rigidbody2D rig;
    bool flag = true;

    float myTime = 0;
    [HideInInspector] public float checkRange;
    [HideInInspector] public float perfectCheckRange;
    bool hadAdd = false, hadRemove = false;

    [Header("��������")]
    public float rotationSpeed = 50f;
    public float scaleSpeed = 0.5f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float targetAlpha = 0.5f;

    private Vector3 initialScale; // ��ʼ����ֵ
    private float currentScaleFactor = 1f; // ��ǰ��������
    private bool scalingUp = true; // �Ƿ����ڷŴ�

    //hold
    [HideInInspector]
    public bool isFirst;
    public float holdTime;
    bool isHolding;
    float isHoldingTime;
    float hitTime;
    bool isFirstFrame = true;
    bool isSpaceReleased = false; // �Ƿ��ɿ��˿ո�
    private Coroutine holdingCoroutine; // ���ڴ洢Э������
    //holdrow
    //bool L2CheckList.headHadMiss = false;


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
        //Debug.Log("checkrange" + checkRange);
        initialScale = transform.localScale;

        myTime = -FindObjectOfType<L2gameController>().moveToAccTime;
        //Debug.Log("myTime" + myTime);
        if (noteTarget == null)
        {
            Debug.LogWarning("������Ŀ�����");
            return;
        }

        // ��ʼ��
        startPosition = transform.position; // ��¼����λ��
        accuratePosition = GetIntersectionPointOnCircle(startPosition, noteTarget.position); // ���㣨ԲȦ�����ϵĵ㣩����
        journeyLength = Vector3.Distance(startPosition, targetPosition); // �������
        accJourneyL = Vector3.Distance(startPosition, accuratePosition);

        // �����ƶ�ʱ��;�������ٶ�
        moveSpeed = accJourneyL / moveToAccTime;

        // ��¼��ʼ�ƶ���ʱ��
        startTime = Time.time;

        rig = GetComponent<Rigidbody2D>();

        if (!isFirst)
        {
            gameObject.transform.localScale *= 0.6f;
        }
        else
        {
            L2CheckList.headHadMiss = false;
        }
    }

    void Update()
    {
        myTime += Time.deltaTime;

        if (isFirst)
        {
            RotateObject();
            ScaleObject();
        }

        //Debug.Log("mytime" + myTime);
        //�����ж�����
        
        if (!hadAdd && myTime > -checkRange / 2)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            //��ԭ��ͣ��
            //if (isMovingOutsideAcc)
            //{
            //    targetPosition = accuratePosition;s
            //}
            //else
            //{
            //    targetPosition = transform.position;
            //}
            //Debug.Log("tap1");
            if (isFirst)
            {
                L2CheckList.headCheckList.Add(this);
                L2CheckList.holdCheckList.Add(this);
                hitTime = myTime;

                //
                if (isHolding)
                {
                    if (isFirstFrame)
                    {
                        // ��һ֡����Ƿ��¿ո�
                        if (Input.GetKey(KeyCode.Space))
                        {
                            isFirstFrame = false;
                        }
                        else
                        {
                            // ��һ֡δ���¿ո񣬱��Ϊδ����
                            isSpaceReleased = true;
                            isFirstFrame = false;

                        }
                    }
                    else
                    {
                        // Hold �׶μ���Ƿ��ɿ��ո�
                        if (!Input.GetKey(KeyCode.Space))
                        {
                            // ��;�ɿ��ո񣬱��Ϊ�ɿ�
                            isSpaceReleased = true;
                        }
                    }
                }
            }
            else
            {
                L2CheckList.holdRow.Add(this);
            }
            hadAdd = true;
        }
        else if (!hadRemove && myTime > checkRange / 2) //���ж�����
        {
            if (isFirst)
            {
                L2CheckList.headCheckList.Remove(this);
                Miss();
            }
            else
            {
                L2CheckList.holdRow.Remove(this);
                Destroy(gameObject);
            }
                hadRemove = true;
        }
        
            //move
        if (isMovingOutsideAcc)
        {
            // �����Ѿ��ƶ���ʱ��
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            // ���㵱ǰ�ƶ��Ľ��ȣ�0 �� 1 ֮�䣩
            float fractionOfAccurateJourney = distanceCovered / accJourneyL;

            if (fractionOfAccurateJourney < 1f)
            {
                transform.position = Vector3.Lerp(startPosition, accuratePosition, fractionOfAccurateJourney);
            }
            else
            {
                // ���� accuratePosition����ʼ�ڶ��׶�
                isMovingOutsideAcc = false;
                //startTime = Time.time; // ���ÿ�ʼʱ��
            }
        }
        else
        {
            // �����Ѿ��ƶ���ʱ��
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            // ���㵱ǰ�ƶ��Ľ��ȣ�0 �� 1 ֮�䣩
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


        //if (!isFirst)
        //{
        //    if (L2CheckList.headHadMiss)
        //    {
        //        //����
        //        Color color = GetComponent<SpriteRenderer>().color;
        //        color.a = targetAlpha;
        //        GetComponent<SpriteRenderer>().color = color;
        //    }
        //    if (L2CheckList.holdCheckList.Count == 0 && L2CheckList.headCheckList.Count == 0)
        //    {
        //        //Destroy(gameObject);
        //    }
        //}
    }

    // �������ɵ㵽Ŀ����������ԲȦ���ߵĽ���
    private Vector3 GetIntersectionPointOnCircle(Vector3 start, Vector3 end)
    {
        // ���㷽������
        Vector3 direction = (start - end).normalized;

        // ���㽻��
        Vector3 intersectionPoint = noteTarget.position + direction * circleRadius;

        return intersectionPoint;
    }

    public bool CheckNote_Head()
    {
        
        hadRemove = true;
        L2CheckList.headCheckList.Remove(this);
        L2CheckList.holdCheckList.Add(this);
        isHolding = true;
        FindObjectOfType<L2gameController>().JudgeNote(hitTime); // �����ж�ʱ��
        //��ԭ��ͣ��
        //targetPosition = accuratePosition;
        //StartCoroutine(HoldingTimer());
        if (this != null ) // �������Ƿ�����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!?????????????????????????
            holdingCoroutine = StartCoroutine(HoldingTimer());
        return true;
    }


    IEnumerator HoldingTimer()
    {
        //while (isHolding)
        //{
        //    isHoldingTime += Time.deltaTime;
        //    isHolding = false;

        //    if (isHoldingTime >= holdTime)
        //    {
        //        L2CheckList.headCheckList.Remove(this);
        //        foreach(L2HoldController h in L2CheckList.holdRow)
        //        {
        //            Debug.Log("destroy");
        //            Destroy(h.gameObject);
        //        }
        //        L2CheckList.holdRow.Clear();
        //        FindObjectOfType<L2gameController>().JudgeNote(hitTime);
        //        //��Ч&����
        //        Destroy(gameObject);
        //        break;
        //    }
        //    yield return 0;
        //}
        //if (isHoldingTime < holdTime)
        //{
        //    Miss();
        //}

        // �ȴ� holdTime ʱ��
        //yield return new WaitForSeconds(holdTime);
        //if (this == null || gameObject == null) // �������Ƿ�����
        //{
        //    yield break; // ����������٣�ֹͣЭ��
        //}
        // �ȴ� holdTime ʱ��
        float elapsedTime = 0f;
        while (elapsedTime < holdTime)
        {
            if (this == null || gameObject == null) // �������Ƿ�����
            {
                yield break; // ����������٣�ֹͣЭ��
            }

            elapsedTime += Time.deltaTime;
            yield return null; // �ȴ���һ֡
        }
        if (this == null || gameObject == null) // �������Ƿ�����
        {
            yield break; // ����������٣�ֹͣЭ��
        }
        // Hold ���
        if (isSpaceReleased)
        {
            // ����ɿ����ո񣬴��� Miss
            Miss();
        }
        else
        {
            // ���һֱ��ס�ո񣬼ӷֲ���������
            //FindObjectOfType<L2gameController>().AddScore(); // �ӷ�
            L2CheckList.holdCheckList.Remove(this); // �� holdCheckList ���Ƴ�
            L2CheckList.headCheckList.Remove(this); // �� holdCheckList ���Ƴ�
            Destroy(gameObject);
        }
    }

    void Miss()
    {
        //if (holdingCoroutine != null)
        //{
        //    StopCoroutine(holdingCoroutine); // ֹͣЭ��
        //}
        isHolding = false;
        Debug.Log("miss");
        //��Ч
        //����
        FindObjectOfType<L2gameController>().MissNote();
        L2CheckList.headCheckList.Remove(this);
        L2CheckList.holdCheckList.Remove(this);
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        //����
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = targetAlpha;
        GetComponent<SpriteRenderer>().color = color;
        /*//����ȫɾ��
        foreach (L2HoldController h in L2CheckList.holdRow)
        {
            Debug.Log("destroy");
            Destroy(h.gameObject);
        }*/
        L2CheckList.holdRow.Clear();
        //L2CheckList.headHadMiss = true;
        Destroy(gameObject);
    }


    private void RotateObject()
    {
        // �� Z ����ת
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void ScaleObject()
    {
        // ������������
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

        // Ӧ������
        transform.localScale = initialScale * currentScaleFactor;
    }

    //void OnDestroy()
    //{
    //    if (holdingCoroutine != null)
    //    {
    //        StopCoroutine(holdingCoroutine);
    //        holdingCoroutine = null;
    //    }
    //    L2CheckList.holdCheckList.Remove(this);
    //    L2CheckList.headCheckList.Remove(this);
    //}
}
