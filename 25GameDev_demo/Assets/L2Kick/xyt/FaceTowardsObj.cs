using UnityEngine;

public class FaceTowardsObj : MonoBehaviour
{
    private Transform target; // 目标物体

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("CatBody").transform;
    }
    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("目标物体未指定！");
            return;
        }

        // 计算方向
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

        // 设置物体旋转角度
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
