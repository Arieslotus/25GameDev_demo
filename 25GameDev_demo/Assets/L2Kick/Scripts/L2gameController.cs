using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class L2gameController : MonoBehaviour
{
    [Header("��ת����")]
    public GameObject Spawner;
    public Transform spawnerTarget; // Χ����ת��Ŀ�����
    public float radius = 2f; // ��ת�뾶
    public float speed = 1f; // ��ת�ٶ�
    [Header("������������")]
    public GameObject notePrefab; // ����Ԥ����
    public Transform spawnPoint; // �������ɵ�λ��
    public float spawnInterval = 1f; // ����������ʱ����

    [Header("Ŀ������")]
    public Transform noteTarget; // Ŀ��㣨���ڴ��ݸ�������������
    public float circleRadius = 2f; // ԲȦ�İ뾶�����ڴ��ݸ�������������
    public float moveTime = 2f; // �����ƶ�ʱ�䣨���ڴ��ݸ�������������

    private float angle = 0f; // ��ǰ��ת�Ƕ�


    // Start is called before the first frame update
    void Start()
    {
        if (notePrefab == null)
        {
            Debug.LogWarning("����������Ԥ���壡");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("��������������λ�ã�");
            return;
        }

        if (noteTarget == null)
        {
            Debug.LogWarning("������Ŀ�����");
            return;
        }

        // ʹ�� InvokeRepeating ÿ��һ��ʱ����������
        InvokeRepeating(nameof(SpawnNote), 0f, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        //Spawn key
        if (spawnerTarget == null)
        {
            Debug.LogWarning("������Ŀ�����");
            return;
        }
        // ������ת�Ƕ�(-˳ʱ��/+��ʱ��)
        angle -= speed * Time.deltaTime;
        // �����µ�λ��
        float x = spawnerTarget.position.x + Mathf.Cos(angle) * radius;
        float y = spawnerTarget.position.y + Mathf.Sin(angle) * radius;
        // ���µ�ǰ�����λ��
        Spawner.transform.position = new Vector3(x, y, transform.position.z);
    }

    public void SpawnNote()
    {
        // ʵ��������Ԥ����
        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);

        // ��ȡ�����Ŀ������ű������ò���
        L2noteController noteController = note.GetComponent<L2noteController>();
        if (noteController != null)
        {
            noteController.noteTarget = noteTarget;
            noteController.circleRadius = circleRadius;
            noteController.moveTime = moveTime;
        }
        else
        {
            Debug.LogWarning("����Ԥ������δ�ҵ� NoteController �ű���");
        }
    }

    // �� Scene ��ͼ�л���ԲȦ��������ԣ�
    private void OnDrawGizmosSelected()
    {
        if (noteTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(noteTarget.position, circleRadius);
        }
    }
}
