using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class L2gameController : MonoBehaviour
{
    [Header("旋转设置")]
    public Transform Spawner;
    public Transform spawnerTarget; // 围绕旋转的目标对象
    public float spawnerRadius = 13f; // 旋转半径
    public float rotationSpeed; // 旋转速度（角度/s）

    private float angle = 0f; // 当前旋转角度
    float startOffsetAngle = -90; //dont edit //起始旋转角偏移90d
    float catRotationOffsetAngle;

    [Header("音符生成设置")]
    public GameObject notePrefab; // 音符预制体
    public float spawnInterval = 1f; // 生成音符的时间间隔

    [Header("目标设置")]
    public Transform noteTarget; // 目标点（用于传递给音符控制器）
    public float accCircleRadius = 2f; // 圆圈的半径（用于传递给音符控制器）
    public float moveToAccTime = 2f; // 音符移动到准确线的时间（用于传递给音符控制器）

    [Header("猫")]
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
        // 使用 InvokeRepeating 每隔一段时间生成音符
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
        // 更新旋转角度(-顺时针/+逆时针)
        angle += rotationSpeed * Time.deltaTime;
        // 计算新的位置
        float x = spawnerTarget.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * spawnerRadius;//Mathf.Cos 和 Mathf.Sin 等三角函数在 Unity 中只接受弧度制作为参数。
        float y = spawnerTarget.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * spawnerRadius;
        // 更新当前对象的位置
        Spawner.position = new Vector3(x, y, transform.position.z);
    }
    private void SpawnNote()
    {
        // 实例化音符预制体
        GameObject note = Instantiate(notePrefab, Spawner.position, Quaternion.identity);

        // 获取音符的控制器脚本并设置参数
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
            Debug.LogWarning("音符预制体上未找到 NoteController 脚本！");
        }
    }

    // 在 Scene 视图中绘制圆圈（方便调试）
    private void OnDrawGizmosSelected()
    {
        if (noteTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(noteTarget.position, accCircleRadius);
        }
    }
}
