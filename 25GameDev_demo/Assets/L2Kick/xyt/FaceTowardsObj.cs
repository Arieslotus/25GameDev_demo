using UnityEngine;

public class FaceTowardsObj : MonoBehaviour
{
    private Transform target; // Ŀ������

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("CatBody").transform;
    }
    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Ŀ������δָ����");
            return;
        }

        // ���㷽��
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

        // ����������ת�Ƕ�
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
