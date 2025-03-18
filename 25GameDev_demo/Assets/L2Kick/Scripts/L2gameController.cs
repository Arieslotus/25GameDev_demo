using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class L2gameController : MonoBehaviour
{
    [Header("��ת����")]
    public Transform Spawner;
    public Transform spawnerTarget; // Χ����ת��Ŀ�����
    public float spawnerRadius = 13f; // ��ת�뾶
    public float rotationSpeed; // ��ת�ٶȣ��Ƕ�/s��

    private float angle = 0f; // ��ǰ��ת�Ƕ�
    float startOffsetAngle = -90; //dont edit //��ʼ��ת��ƫ��90d
    float catRotationOffsetAngle;

    [Header("������������")]
    public GameObject notePrefab; // ����Ԥ����
    public float spawnInterval = 1f; // ����������ʱ����

    [Header("Ŀ������")]
    public Transform noteTarget; // Ŀ��㣨���ڴ��ݸ�������������
    public float accCircleRadius = 2f; // ԲȦ�İ뾶�����ڴ��ݸ�������������
    public float moveToAccTime = 2f; // �����ƶ���׼ȷ�ߵ�ʱ�䣨���ڴ��ݸ�������������

    [Header("è")]
    public CatController cat;

    private void Awake()
    {
        //rotate           
        catRotationOffsetAngle = -moveToAccTime * rotationSpeed;

        angle += startOffsetAngle;
        cat.offsetAngle = catRotationOffsetAngle;

        SpawnerRote();
    }

    void Start()
    {
        // ʹ�� InvokeRepeating ÿ��һ��ʱ����������
        InvokeRepeating(nameof(SpawnNote), 0f, spawnInterval);

        //cat
        cat.rotationSpeed = rotationSpeed;
    }


    void Update()
    {
        SpawnerRote();
    }

    void SpawnerRote()
    {
        // ������ת�Ƕ�(-˳ʱ��/+��ʱ��)
        angle += rotationSpeed * Time.deltaTime;
        // �����µ�λ��
        float x = spawnerTarget.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * spawnerRadius;//Mathf.Cos �� Mathf.Sin �����Ǻ����� Unity ��ֻ���ܻ�������Ϊ������
        float y = spawnerTarget.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * spawnerRadius;
        // ���µ�ǰ�����λ��
        Spawner.position = new Vector3(x, y, transform.position.z);
    }
    private void SpawnNote()
    {
        // ʵ��������Ԥ����
        GameObject note = Instantiate(notePrefab, Spawner.position, Quaternion.identity);

        // ��ȡ�����Ŀ������ű������ò���
        L2noteController noteController = note.GetComponent<L2noteController>();
        if (noteController != null)
        {
            noteController.noteTarget = noteTarget;
            noteController.circleRadius = accCircleRadius;
            noteController.moveToAccTime = moveToAccTime;
            noteController.targetPosition = cat.transform.position;
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
            Gizmos.DrawWireSphere(noteTarget.position, accCircleRadius);
        }
    }
}
