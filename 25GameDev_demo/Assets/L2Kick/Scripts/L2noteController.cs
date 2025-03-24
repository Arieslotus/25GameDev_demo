using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class L2noteController : MonoBehaviour
{
    [Header("Ŀ������")]
    [HideInInspector]public Transform noteTarget; // Ŀ���
    [HideInInspector]public float circleRadius = 2f; // ԲȦ�İ뾶

    [Header("�ƶ�����")]
    [HideInInspector]public float moveToAccTime; // ���������ɵ㵽ԲȦ���ߵ�ʱ��
    private float moveSpeed; // ��������ƶ��ٶ�

    private Vector3 startPosition; // ����������λ��
    [HideInInspector] public Vector3 targetPosition; // ������Ŀ��λ��
    private Vector3 accuratePosition;
    private float journeyLength,accJourneyL; // ���ɵ㵽Ŀ���ľ���
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
    // �����ٶ�
    public float scaleSpeed = 0.5f;
    // ���ŷ�Χ����С��������ű�����
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    private Vector3 initialScale; // ��ʼ����ֵ
    private float currentScaleFactor = 1f; // ��ǰ��������
    private bool scalingUp = true; // �Ƿ����ڷŴ�

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
    }

    void Update()
    {
        myTime += Time.deltaTime;

        RotateObject();
        ScaleObject();

        //Debug.Log("mytime" + myTime);
        //�����ж�����
        if (!hadAdd && myTime > -checkRange / 2)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            //Debug.Log("tap1");
            L2CheckList.tapCheckList.Add(this);
            hadAdd = true;
        }
        else if (!hadRemove && myTime > checkRange/2) //���ж�����
        {
            L2CheckList.tapCheckList.Remove(this);
            hadRemove = true;
            Miss();
        }

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

    public bool CheckNote_Tap()
    {
        //Debug.Log("tap");
        FindObjectOfType<L2gameController>().JudgeNote(myTime);
        L2CheckList.tapCheckList.Remove(this);
        Destroy(gameObject);
        return true;
    }

    void Miss()
    {
        //Debug.Log("miss");
        //��Ч
        //����
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
        // �� Z ����ת
        transform.Rotate(0,0, rotationSpeed * Time.deltaTime);
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
}
