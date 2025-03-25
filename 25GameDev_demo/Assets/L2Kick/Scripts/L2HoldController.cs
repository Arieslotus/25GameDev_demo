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
    public bool isFirst = false;
    public float holdTime;
    bool isHolding = false;
    float isHoldingTime;
    float hitTime;
    bool isFirstFrame = true;
    public bool isSpaceReleased = false; // �Ƿ��ɿ��˿ո�
    private Coroutine holdingCoroutine; // ���ڴ洢Э������
    bool hadSetAlpha = false;
    //holdrow
    //bool L2CheckList.headHadMiss = false;

    [Header("��Ч")]
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
        //�����ж�����
        
        if ( myTime > -checkRange / 2 && myTime < checkRange / 2)
        {
            //Debug.Log("����Χ");
            if (!hadAdd)//ִ��һ��
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
            else //ѭ��
            {
                if (isFirst)
                {
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        // ��;�ɿ��ո񣬱��Ϊ�ɿ�
                        isSpaceReleased = true;
                        myFootStatus = footStatus.none;
                        //L2CheckList.isSpaceReleased = true;

                    }
                }
                else
                {
                    if (L2CheckList.headHadMiss && !hadSetAlpha)
                    {
                        //����
                        //Debug.Log("����");
                        //StopPerfectEffect();
                        Color color = GetComponent<SpriteRenderer>().color;
                        color.a = 0.1f;
                        GetComponent<SpriteRenderer>().color = color;
                        hadSetAlpha = true;
                    }
                }
            }
        }
        else if (myTime > checkRange / 2) //���ж�����,ײ��è
        {
            //Debug.Log("����Χ");
            if (!hadRemove)
            {
                //Debug.Log("����Χ");
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

                //�������hold��destroy�߼�
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

    public bool CheckNote_Head() //�Ѿ�����
    {
        hadRemove = true;
        isHolding = true;
        L2CheckList.headHadMiss = false;
        myFootStatus = footStatus.perfect;
        //������Ч
        PlayPerfectEffect();

        FindObjectOfType<L2gameController>().JudgeNote(hitTime); // �����ж�ʱ��
        if (this != null ) // �������Ƿ�����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!?????????????????????????
            holdingCoroutine = StartCoroutine(HoldingTimer());
        return true;
    }


    IEnumerator HoldingTimer()
    {
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
            //Debug.Log("finish+stop effect");
            myFootStatus = footStatus.none;
            //StopPerfectEffect();
            L2CheckList.headCheckList.Remove(this); // �� holdCheckList ���Ƴ�
            Destroy(gameObject);
        }
    }

    void Miss()
    {
        L2CheckList.headHadMiss = true;
        //Debug.Log("L2CheckList.headHadMiss" + L2CheckList.headHadMiss);
        //Debug.Log("miss");
        //��Ч
        //����
        myFootStatus = footStatus.none;
        //StopPerfectEffect();
        FindObjectOfType<L2gameController>().MissNote();
        L2CheckList.headCheckList.Remove(this);
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

    void OnDestroy()
    {
        if (isFirst)
            Debug.Log("firstDestroyed");
    }

    void PlayPerfectEffect()
    {
        //Debug.Log("ins hold effect");
        //perfectHold = Instantiate(perfectHoldPre, transform.position, Quaternion.identity);
        //perfectHoldEffect = perfectHold.GetComponentsInChildren<ParticleSystem>();
        smoke = Instantiate(smokePre, transform.position, Quaternion.identity);
        smokeEffect = smoke.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in smokeEffect)
        {
            ps.Play();
        }
        //foreach (ParticleSystem ps in perfectHoldEffect)
        //{
        //    ps.Play();
        //}
        ////Destroy(smoke, 2f);
        //Instantiate(smokePre, transform.position, Quaternion.identity);
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
                // ��������ϵͳλ��
                ps.transform.position = position;
            }
        }
    }
}
