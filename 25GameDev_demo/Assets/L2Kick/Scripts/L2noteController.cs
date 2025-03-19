using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2noteController : MonoBehaviour
{
    [Header("Ŀ������")]
    public Transform noteTarget; // Ŀ���
    public float circleRadius = 2f; // ԲȦ�İ뾶
    private Vector3 noteTargetPosition;

    [Header("�ƶ�����")]
    public float moveTime = 2f; // ���������ɵ㵽ԲȦ���ߵ�ʱ��
    private float moveSpeed; // ��������ƶ��ٶ�

    private Vector3 startPosition; // ����������λ��
    private Vector3 targetPosition; // ������Ŀ��λ�ã�ԲȦ�����ϵĵ㣩
    private float journeyLength; // ���ɵ㵽Ŀ���ľ���
    private float journeyLengthTotal; // ���ɵ㵽Ŀ���ľ���
    private float startTime; // ������ʼ�ƶ���ʱ��

    void Start()
    {
        if (noteTarget == null)
        {
            Debug.LogWarning("������Ŀ�����");
            return;
        }

        // ��ʼ��
        startPosition = transform.position; // ��¼����λ��
        targetPosition = GetIntersectionPointOnCircle(startPosition, noteTarget.position); // ���㽻��
        journeyLength = Vector3.Distance(startPosition, targetPosition); // �������
        journeyLengthTotal = Vector3.Distance(startPosition, noteTargetPosition); // �������
        noteTargetPosition = noteTarget.position;
        gameObject.GetComponent<Collider2D>().enabled = false;

        // �����ƶ�ʱ��;�������ٶ�
        moveSpeed = journeyLength / moveTime;

        // ��¼��ʼ�ƶ���ʱ��
        startTime = Time.time;
    }

    void Update()
    {
        if (noteTarget == null) return;

        // �����Ѿ��ƶ���ʱ��
        float distanceCovered = (Time.time - startTime) * moveSpeed;

        // ���㵱ǰ�ƶ��Ľ��ȣ�0 �� 1 ֮�䣩
        //float targetFractionOfJourney = distanceCovered / journeyLength;

        float fractionOfJourney = distanceCovered / journeyLengthTotal;

        // ʹ�� Lerp ƽ���ƶ�
        transform.position = Vector3.Lerp(startPosition, noteTargetPosition, fractionOfJourney);

        // �������Ŀ������
        if (fractionOfJourney >= journeyLength/journeyLengthTotal)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        //�����������
        if (fractionOfJourney >= 1f)
        {
            //Destroy(gameObject);
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
    private void OnDrawGizmosSelected()
    {
        if (noteTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(noteTarget.position, circleRadius);
        }
    }

    //��ҵ㰴
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
