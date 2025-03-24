using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public GameObject tap; // 音符预制体
    public GameObject hold; // 音符预制体
    public GameObject head;
    //public float spawnInterval = 1f; // 生成音符的时间间隔
    public float spawnInterval = 0.1f; // 生成 Note 的时间间隔


    [Header("目标设置")]
    public Transform noteTarget; // 目标点（用于传递给音符控制器）
    public float accCircleRadius = 2f; // 圆圈的半径（用于传递给音符控制器）
    public float moveToAccTime = 2f; // 音符移动到准确线的时间（用于传递给音符控制器）

    [Header("猫")]
    public CatController cat;

    [Header("音乐与谱面")]
    public AudioSource musicPlayer;
    public TextAsset chart;
    private float[] timeStamps;
    private int[] noteType;//1tap, 2hold
    private float[] noteHoldTime;

    [Header("音符判定设置")]
    public float checkTimeRange = 0.16f;
    public float perfectCheckTimeRange = 0.08f;

    [HideInInspector]public float myTime = 0;//计时器
    private int index = 0;

    [Header("积分")]
    [HideInInspector] public int health;
    [HideInInspector] public int score;
    [HideInInspector] public int combo;
    [HideInInspector] public int totalScore;
    public GameObject UI_combo;
    public GameObject UI_health;
    public Text text_score;
    public Text text_health;
    public Text text_combo;
    public int scorePerPerfect;
    public int scorePerGood;
    public int scorePerMiss;
    public int numToShowCombo;

    [Header("判定")]
    public GameObject perfect;
    public GameObject good;
    public GameObject miss;
    public Transform spawnPos;
    //public float riseSpeed = 1f; // 上浮速度
    public float fadeOutDuration = 1f; // 淡出持续时间
    public float destroyDelay = 1f; // 销毁延迟时间
    public float riseDuration = 1f; // 上浮持续时间
    public float pauseDuration = 0.5f; // 停顿时间
    public float targetHeight = 2f; // 停顿时的高度
    public float continueRiseSpeed = 2f;
    public bool startSpawn = false;

    [Header("游戏结束")]
    public GameObject gamefailed;
    public GameObject gameCompleted;
    public GameObject resultPage;
    public Image result;
    public Sprite S;
    public Sprite A;
    public Sprite B;







    private void Awake()
    {
        LoadChart();
        musicPlayer.Pause();
        StartCoroutine("GameStart");
        //对齐延时（-3+2）
        myTime = -3 + moveToAccTime;

        //rotate           
        catRotationOffsetAngle = -moveToAccTime * rotationSpeed;

        angle += startOffsetAngle;
        cat.offsetAngle = catRotationOffsetAngle;

        SpawnerRote();
    }

    void Start()
    {
        // 使用 InvokeRepeating 每隔一段时间生成音符
        //InvokeRepeating(nameof(SpawnNote), 0f, spawnInterval);

        //cat
        cat.rotationSpeed = rotationSpeed;
        score = 0;
        health = 3;
        UI_combo.SetActive(false);
    }


    void Update()
    {
        SpawnerRote();
        myTime += Time.deltaTime;
        if (index < timeStamps.Length)
        {
            if (myTime >= timeStamps[index])
            {
                Debug.Log("noteType[index]" + noteType[index]);
                //Instantiate(tap, Spawner.position, Quaternion.identity);
                if (noteType[index] == 1)
                {
                    Debug.Log("Spawn Tap");
                    SpawnNote();
                }
                else if (noteType[index] == 2)
                {
                    SpawnHoldNote(index);
                }

                index++;
            }

        }

        //test
        if (startSpawn)
        {
            if (myTime > 0.8)
            {
                Instantiate(perfect,spawnPos);
                myTime = 0;
            }
        }

        if (health <= 0)
        {
            ////失败提示
            //StartCoroutine(EndWithDelay(false));
            //enabled = false; // 禁用 Update 检测
        }


        //音乐播放完毕
        if (musicPlayer.isPlaying && musicPlayer.time > musicPlayer.clip.length)
        {
            Debug.Log("music over");
            //成功提示
            StartCoroutine(EndWithDelay(true));
            enabled = false; // 禁用 Update 检测
        }
    }

    IEnumerator EndWithDelay(bool isCompleted)
    {
        yield return new WaitForSeconds(2); // 等待延迟时间

        result.gameObject.SetActive(true);
        //if (isCompleted)
        //{
        //    result.sprite = resultImage_Win;
        //}
        //else
        //{
        //    result.sprite = resultImage_Failed;
        //}
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(3);
        musicPlayer.Play();
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
        GameObject note = Instantiate(tap, Spawner.position, Quaternion.identity);
    }

    void SpawnHoldNote(int index)
    {
        StartCoroutine(SpawnHoldNotesCoroutine(index));
    }
    private IEnumerator SpawnHoldNotesCoroutine(int index)
    {
        float elapsedTime = 0f; // 已过去的时间

        while (elapsedTime < noteHoldTime[index]- spawnInterval)
        {
            //Debug.Log("startSpawnHold");
            //首个hold
            if (elapsedTime ==0f)
            {
               // Debug.Log("spawnHead");
                GameObject note = Instantiate(head, Spawner.position, Quaternion.identity);
                note.GetComponent<L2HoldController>().holdTime = noteHoldTime[index];
                note.GetComponent<L2HoldController>().isFirst = true;
            }
            else
            {
                //Debug.Log("spawnHold");
                GameObject note = Instantiate(hold, Spawner.position, Quaternion.identity);
                note.GetComponent<L2HoldController>().isFirst = false;
            }

            // 等待 spawnInterval 时间
            yield return new WaitForSeconds(spawnInterval);

            // 更新已过去的时间
            elapsedTime += spawnInterval;
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

    void LoadChart()
    {
        string[] eachLine = chart.text.Split("\n");
        timeStamps = new float[eachLine.Length];
        noteType = new int[eachLine.Length];
        noteHoldTime = new float[eachLine.Length];
        for(int i = 0; i < eachLine.Length; i++)
        {
            string[] eachPart = eachLine[i].Split(",");
            //tap
            if (eachPart.Length==2)
            {
                timeStamps[i] = Convert.ToSingle(eachPart[0]);
                noteType[i] = 1;
                totalScore += scorePerPerfect;
            }
            //hold
            else if (eachPart.Length==3)
            {
                timeStamps[i] = Convert.ToSingle(eachPart[0]);
                noteType[i] = 2;
                noteHoldTime[i] = Convert.ToSingle(eachPart[2]);
            }
            
            //noteType = Convert.ToSingle(eachPart[1]);
        }
    }

    public void JudgeNote(float hitTime)
    {
        hitTime = System.Math.Abs(hitTime);
        if (hitTime < perfectCheckTimeRange / 2) //perfect
        {
            score += scorePerPerfect;
            if (L2CheckList.graderList != null && L2CheckList.graderList.Count > 2)
            {
                foreach (var grader in L2CheckList.graderList)
                {
                    if (grader != null)
                    {
                        Destroy(grader.gameObject);
                    }
                }

                // 清空列表
                L2CheckList.graderList.Clear();
            }
            Instantiate(perfect,spawnPos);
        }
        else
        {
            score += scorePerGood;
            if (L2CheckList.graderList != null && L2CheckList.graderList.Count > 2)
            {
                foreach (var grader in L2CheckList.graderList)
                {
                    if (grader != null)
                    {
                        Destroy(grader.gameObject);
                    }
                }

                // 清空列表
                L2CheckList.graderList.Clear();
            }
            Instantiate(good,spawnPos);
        }
        combo++;
        if (combo > numToShowCombo)
        {
            UI_combo.SetActive(true);
        }
        text_score.text = "" + score;
        text_combo.text = "" + combo;
    }

    public void MissNote()
    {
        combo = 0;
        score += scorePerMiss;
        health--;
        if (UI_health.transform.childCount > 0)
        {
            // 获取第一个子物体
            Transform firstChild = UI_health.transform.GetChild(0);
            // 销毁第一个子物体
            Destroy(firstChild.gameObject);
        }
        text_score.text = "" + score;
        text_health.text = "health" + health;
        if (L2CheckList.graderList != null && L2CheckList.graderList.Count > 2)
        {
            foreach (var grader in L2CheckList.graderList)
            {
                if (grader != null)
                {
                    Destroy(grader.gameObject);
                }
            }

            // 清空列表
            L2CheckList.graderList.Clear();
        }
        Instantiate(miss,spawnPos);
        UI_combo.SetActive(false);
    }
}
