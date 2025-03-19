using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
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
        if(isMovingOutsideAcc)
        {
            // �����Ѿ��ƶ���ʱ��
            float distanceCovered = (Time.time - startTime) * moveSpeed;

            // ���㵱ǰ�ƶ��Ľ��ȣ�0 �� 1 ֮�䣩
            float fractionOfAccurateJourney = distanceCovered / accJourneyL;

            // ʹ�� Lerp ƽ���ƶ�
            transform.position = Vector3.Lerp(startPosition, accuratePosition, fractionOfAccurateJourney);

            // �������׼ȷ����
            if (fractionOfAccurateJourney >= 1f)
            {
                isMovingOutsideAcc = false;
            }
        }
        else
        {
            // �����Ѿ��ƶ���ʱ��
            float distanceCovered = (Time.time - startTime) * moveSpeed;

            // ���㵱ǰ�ƶ��Ľ��ȣ�0 �� 1 ֮�䣩
            float fractionOfJourney = distanceCovered / journeyLength;

            // ʹ�� Lerp ƽ���ƶ�
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
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

    // �� Scene ��ͼ�л���ԲȦ��������ԣ�
    //private void OnDrawGizmosSelected()
    //{
    //    if (noteTarget != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(noteTarget.position, circleRadius);
    //    }
    //}

    //��ҵ㰴
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);

        if (collision.CompareTag("CatBody"))
        {
            //С���è����(��)
            Destroy(gameObject);
            //Debug.Log("bump body");
            //if (collision.GetComponent<SpriteRenderer>())
            //{
            //    collision.GetComponent<SpriteRenderer>().color = Color.red;
            //}
        }

        if (collision.CompareTag("CatLeg"))
        {
            //С���è�ȣ�ɨ�ȣ�
            Destroy(gameObject);
            Debug.Log("bump leg");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "CatFoot")
        {
            //���ߵ�С��Ŀǰת��-200�����������Ҵ��С����Ȼ���ڽŴ��������
            Destroy(gameObject);
            //Debug.Log("bump foot");
        }
    }
}
