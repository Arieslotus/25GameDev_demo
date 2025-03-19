using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class L2gameController : MonoBehaviour
{
    [Header("旋转设置")]
    public GameObject Spawner;
    public Transform spawnerTarget; // 围绕旋转的目标对象
    public float radius = 2f; // 旋转半径
    public float speed = 1f; // 旋转速度
    [Header("音符生成设置")]
    public GameObject notePrefab; // 音符预制体
    public Transform spawnPoint; // 音符生成的位置
    public float spawnInterval = 1f; // 生成音符的时间间隔

    [Header("目标设置")]
    public Transform noteTarget; // 目标点（用于传递给音符控制器）
    public float circleRadius = 2f; // 圆圈的半径（用于传递给音符控制器）
    public float moveTime = 2f; // 音符移动时间（用于传递给音符控制器）

    private float angle = 0f; // 当前旋转角度


    // Start is called before the first frame update
    void Start()
    {
        if (notePrefab == null)
        {
            Debug.LogWarning("请设置音符预制体！");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("请设置音符生成位置！");
            return;
        }

        if (noteTarget == null)
        {
            Debug.LogWarning("请设置目标对象！");
            return;
        }

        // 使用 InvokeRepeating 每隔一段时间生成音符
        InvokeRepeating(nameof(SpawnNote), 0f, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        //Spawn key
        if (spawnerTarget == null)
        {
            Debug.LogWarning("请设置目标对象！");
            return;
        }
        // 更新旋转角度(-顺时针/+逆时针)
        angle -= speed * Time.deltaTime;
        // 计算新的位置
        float x = spawnerTarget.position.x + Mathf.Cos(angle) * radius;
        float y = spawnerTarget.position.y + Mathf.Sin(angle) * radius;
        // 更新当前对象的位置
        Spawner.transform.position = new Vector3(x, y, transform.position.z);
    }

    public void SpawnNote()
    {
        // 实例化音符预制体
        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);

        // 获取音符的控制器脚本并设置参数
        L2noteController noteController = note.GetComponent<L2noteController>();
        if (noteController != null)
        {
            noteController.noteTarget = noteTarget;
            noteController.circleRadius = circleRadius;
            noteController.moveTime = moveTime;
        }
        else
        {
            Debug.LogWarning("音符预制体上未找到 NoteController 脚本！");
        }
    }

    // 在 Scene 视图中绘制圆圈（方便调试）
    private void OnDrawGizmosSelected()
    {
        if (noteTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(noteTarget.position, circleRadius);
        }
    }
}
